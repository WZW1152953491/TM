using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;

namespace TMCurve.MyClass
{
    /*实时数据导入之DTS:
     需要的数据有，原文件夹，目标文件夹，对比时间间隔t，导入时间间隔T；
     实现方法：
    1  建立DTStable,用于存储数据。
    2  移动文件。
    3 将DTStable的存入list<DataTable>中
    4 循环list进行画图
    */
    class importDTSreal : MyClass
    {
        //实例创建一个新表用于存储路径的表
        public static DataTable DTStable = getDTStable();
        public static DataTable getDTStable()//
        {
            DataTable table = new DataTable();
            table.Columns.Add("fa");
            return table;
        }
        //建立一个变量，用于计数，为每隔固定有效文件数量后存入数据到数据库
        public static int coun = 0;
        //新建一个list 用于存储datatable数据。
        private static List<DataTable> list = new List<DataTable>();
        private static List<DataTable> listdraw = new List<DataTable>();
        //监控文件夹文件变化的方法
        public static  void jiankong()
        {
            var m_Watcher = new System.IO.FileSystemWatcher();
            m_Watcher.Path = MainForm.getInstance().realDTS1.Text;
            m_Watcher.Filter = "*.tra";//设置监控的文件名称
            m_Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite |NotifyFilters.FileName |NotifyFilters.DirectoryName;
            m_Watcher.Created += m_Watcher_Changed;//当出现创建（或是移动到或是粘贴）“.tra”文件时发生的时间
            m_Watcher.IncludeSubdirectories = true;//指定是否监控子目录
            m_Watcher.EnableRaisingEvents = true;//是否开始监控（该监控控件是否开始运行）
            
            //m_Watcher.Changed += m_Watcher_Changed;
            //m_Watcher.Deleted += m_Watcher_Changed;
            //m_Watcher.Renamed += m_Watcher_Changed;
        }
        //当出现文件变动时 发生的事情。
       private static void m_Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            //MessageBox.Show("文件被监控到了");
            System.Threading.Thread.Sleep(5000);
            if (listdraw.Count == 0)
            {
                DateTime t = MainForm.getInstance().jizhunTime.Value;
                DataTable jizhun = getJtable(t);
                if (jizhun!=null)
                {
                    listdraw.Add(jizhun);
                }
             }
             
           
            //MessageBox.Show("文件写入完成了");
            try 
            {
                string NewFile = moveFile(e.FullPath, MainForm.getInstance().realDTS2.Text);//移动文件并得到新的路径
                //将数据添加到list中，将路径添加到table中
                getDrawTable(NewFile);
                //存数据到list
                if (list.Count <=10){}
                else
                {
                    /*
                     *1 若数据正常达到11的话，删除最初的列表。并作图。
                     *2 若数据不正常，则list不会增加到11 coun也不会变化，会继续判断list.Count
                     *是否为0，此时肯定不为0，而是10（其实是已经使用过的数据）
                     *则绘图图形不变，若导入，判断是否重复（重复）
                     */
                    list.RemoveAt(0);
                    DTStable.Rows[0].Delete();
                    coun++;
                    if (coun > 9)
                    {
                        coun = 0;
                        string name = DTStable.Rows[0][0].ToString();
                        importDTSReal(list[0], name);//需要去重复，以为list[]不变，下面代码还会做一次对比，然后数据库导入
                    }
                    
                }
                if (list.Count != 0)//防止第一个文件是空的
                {
                    DataTable jizhun1 = MyDataTable.getTraDatatable(NewFile);//最新存数据到内存表
                    importRealDTS(list);
                    if(jizhun1.Rows.Count!=0)
                    {

                        if (listdraw.Count != 0)
                        {

                            string Str = null;
                            float yujingTM = getFloat(listdraw[0], jizhun1);
                            int du1 = 0;
                            if (MainForm.getInstance().yujignTMC.Text != "")
                            {
                                du1 = Convert.ToInt32(MainForm.getInstance().yujignTMC.Text);//对比的数值；
                            }
                            if (yujingTM > du1)
                            {

                                //最多画两个图
                                listdraw.Add(jizhun1);
                                if (listdraw.Count == 2)//表示开始进行蒸汽上返预警
                                {
                                    Str = "基准时间点：" + listdraw[0].Rows[0][0].ToString() + " 蒸汽上返异常开始时间点：" + jizhun1.Rows[0][0].ToString();
                                }
                                if (listdraw.Count == 4)
                                {
                                    listdraw.RemoveAt(2);
                                }
                                /*警报发出来，并且给出警报，并且完成*/
                                if (Str != null)
                                {
                                    shangfan(Str, yujingTM);
                                }
                                drawingRealDTS.drawRealDTS(listdraw);
                            }
                            else
                            {
                                //最多画一个图
                                if (listdraw.Count == 3)
                                {
                                    listdraw.RemoveAt(2);//必须2 咋前面删除
                                    listdraw.RemoveAt(1);
                                
                                    Str = "基准时间点：" + listdraw[0].Rows[0][0].ToString() + " 蒸汽上返异常结束时间点：" + jizhun1.Rows[0][0].ToString();
                                }
                                if (listdraw.Count == 2)
                                {
                                    listdraw.RemoveAt(1);
                                    Str = "基准时间点：" + listdraw[0].Rows[0][0].ToString() + " 蒸汽上返异常结束时间点：" + jizhun1.Rows[0][0].ToString();
                                    // listdraw.RemoveAt(2);
                                }
                                //listdraw.Add(jizhun1);
                                //drawingRealDTS.drawRealDTS(listdraw);
                                if (Str != null)
                                {
                                    shangfan(Str, yujingTM);
                                }
                                drawingRealDTS.drawRealDTS1(jizhun1);
                            }
                        }
                          
                        else
                        {
                            drawingRealDTS.drawRealDTS1(jizhun1);
                        }

                    }
                   
                   
                    
                    if (importGratreal.Gratlist.Count != 0)
                    {
                        DTSFBGdata();//数据对比
                    }
                    else
                    {
                        //两个list任意一个为空，就不进行数据对比。
                    }
               
                }
 
            }
            catch (Exception ex)
            {
                MessageBox.Show("实时监控出错，请联系开发人员" + ex);
                //writelog.WriteLog("" + ex);
            }
           
        }
       public static void  importRealDTS(List<DataTable> dt)//依靠内存list存入数据库，先创建表
       {
           int du = 9;
           int du1 = 5;
           if (MainForm.getInstance().realDTS3.Text!="")
           {
                 du = Convert.ToInt32(MainForm.getInstance().realDTS3.Text);//对比的时间间隔
           }
           if (MainForm.getInstance().realDTS4.Text != "")
           {
               du1 = Convert.ToInt32(MainForm.getInstance().realDTS4.Text);//对比的数值；
           }
          
           
           if (dt.Count <= 9)
           {
               
               //存入数据库（dt[dt.Count-1]）前10个文件全部存入
               importDTSReal(dt[dt.Count - 1], DTStable.Rows[dt.Count - 1][0].ToString());
           }
           else {              
             
               //if(list.Count==10){
                   DataTable table1 = dt[dt.Count - 1];
                   string name = DTStable.Rows[dt.Count - 1][0].ToString();
                   DataTable table2 = dt[dt.Count - 1 - du];
                   float max = getFloat(table1, table2);
                   MainForm.getInstance().DTSdifMax.Text = max.ToString();
                   if (max > du1)
                   {
                       //存入数据库（table1）
                       importDTSReal(table1, name);
                   }
                   else
                   {
                       //不是特殊的文档不存入数据库不存入
                   }
               //}
            
           }

       }
       public static float getFloat(DataTable a, DataTable b)//获取两个文件对比的最大
       {
           int Counta = a.Rows.Count;
           int Countb = b.Rows.Count;
           int Countc = Math.Min(Counta, Countb);
           float maxa = 0;
           for (int i = 0; i < Countc; i++)
           {
              
               float aa45 = float.Parse(b.Rows[i][2].ToString()) - float.Parse(a.Rows[i][2].ToString());
               aa45 = System.Math.Abs(aa45);
               if (aa45 > maxa)
               {
                   maxa = aa45;
               }
           }
          
           return maxa;

       }
        /*文件移动，另创建特定的、以天为分割的文件夹*/
        public static string  moveFile( string OrignFile, string  NewFile)
        {
            string timestr = OrignFile.Substring(OrignFile.LastIndexOf("\\") + 1, OrignFile.LastIndexOf(".") - (OrignFile.LastIndexOf("\\") + 1));//去除后缀
            string str = timestr.Remove(0, timestr.Length - 14);//取去除后缀的文件名称的后14位数据。
            str = str.Remove(8);
            NewFile = NewFile + "\\" + str;
            if (!Directory.Exists(NewFile))//判断文件夹是否存在，不存在则创建
            {
                Directory.CreateDirectory(NewFile);
            }
            NewFile = NewFile + "\\" + timestr + ".tra";
            try
            {
                File.Move(OrignFile, NewFile);
            }
            catch //(Exception EX) 
            {
              //  MessageBox.Show("移动失败+ex");
                //writelog.WriteLog(""+EX); 
            }
            //writelog.WriteLog("" + NewFile);
            return NewFile;
        }

        /*向表中添加路径信息*/
        private static void getDrawTable(string filename)
        {
            DataTable ob = MyDataTable.getTraDatatable(filename);//存数据到内存表
            if (ob.Rows.Count==0)//如果是空的table则不存入列表
            {
             
                string timestr = filename.Substring(filename.LastIndexOf("\\") + 1, filename.LastIndexOf(".") - (filename.LastIndexOf("\\") + 1));//去除后缀
                string str = timestr.Remove(0, timestr.Length - 14);//取去除后缀的文件名称的后14位数据。
                DateTime dt = DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);     
                //如果文件数据未能存入内存表（如文件数据错误等原因），不进行数据存储到list中
                //bool a = true;
                //MainForm.getInstance().errorDTS.Text = "警告："+dt.ToString()+"的文档不正确（乱码）！";
                //MainForm.getInstance().errorDTS.Visible = true;
               
            }
            else
            {
                MainForm.getInstance().errorDTS.Text = " ";
                //MainForm.getInstance().errorDTS.Visible = false;
                list.Add(ob);
                DataRow dr = DTStable.NewRow();//创建数据行
                dr["fa"] = filename;
                DTStable.Rows.Add(dr);//将创建的数据行添加到table中
            
            }
           
        }
       
        public static void importDTSReal(DataTable table,string filename )
        {
            try {
                MySqlConnection mycon = getMycon();
                DateTime dt = Convert.ToDateTime(table.Rows[0][0]);
                //判断是否已经导入数据库
                string Str = "select COUNT(*) from  alltemporpary_data where  folderTime='" + dt + "'";//如果等于‘1’下面的代码不执行。
                MySqlCommand mycmd = new MySqlCommand(Str, mycon);
                int count = Convert.ToInt32(mycmd.ExecuteScalar());
                if (count != 0)
                {

                }
                else
                {
                    string name = CreateTable.getTableName(dt, "DTS");//获取文件将要存入的表名称
                    CreateTable.CDataTable(name);//创建表

                    string str, sum = null;
                    //try
                    //{
                        for (int h = 0; h < table.Rows.Count; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                        {
                            str = "('" + dt + "','" + float.Parse(table.Rows[h][1].ToString()) + "','" + float.Parse(table.Rows[h][2].ToString()) + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
                            sum = sum + str;
                        }
                        string str1 = "insert into " + name + "(RecordTime,Depth,TM) values" + sum;
                        string str2 = str1.Substring(0, str1.LastIndexOf(","));//获取SQL语句
                        doStrmycon(str2, mycon);
                        filename = filename.Replace("\\", "\\\\");//为了保存路径到数据库，不许进行的操作。
                        string str3 = "insert into alltemporpary_data(folderUrl,folderTime,folderTable) values ('" + filename + "','" + dt + "','" + name + "')";
                        doStrmycon(str3, mycon);
                    //}
                    //catch (Exception se)
                    //{
                    //    writelog.WriteLog("部分文件夹或文件不可读" + se);
                    //}

                }
                mycon.Close();
                mycon.Dispose();
            }
            catch //(Exception se)
            {
                //writelog.WriteLog("部分文件夹或文件不可读" + se);
            }
        }
        //public static void importReal()
        //{
        //    if (DTStable.Rows.Count < 1)
        //    {
        //        //table.add
        //        //画图
        //        //导入数据库
        //    }
        //    else
        //    {
        //        //if (DTStable.Rows[0][0] - DTStable.Rows[DTStable.Rows.Count - 1][0] > 5)

        //        //{

        //        //}
        //    }
        //}
        public static void houerimport()
        {

        }
        public static void DTSFBGdata()
        {
            float num1 = 5;
            if (MainForm.getInstance().textBox14.Text != "")
            {
                num1 = int.Parse(MainForm.getInstance().textBox14.Text);
            }
            DataTable t1 = list[list.Count - 1];
            string T1 = t1.Rows[0][0].ToString();
            DataTable t2 = importGratreal.Gratlist[importGratreal.Gratlist.Count - 1];
            string T2 = t2.Rows[0][0].ToString();

            float num = getNewFloat(t1, t2);

            string str = "初始点：DTS" + T1 + "对比点：FBG" + T2;
            if (num > num1)
            {
                if (MainForm.getInstance().datajilu.Items.Count > 100)
                {
                    MainForm.getInstance().datajilu.Items.RemoveAt(0);
                }
                else
                {

                }
                MainForm.getInstance().errorbt.BackColor = Color.Red;
                MainForm.getInstance().errorbt.Text = "点击查看警报";
                MainForm.getInstance().datajilu.Items.Add("记录" + str + "温度差是" + num);
            }
            else
            {

            }

        }
        public static void shangfan(string Str,float num)
        {


            if (MainForm.getInstance().shangfanerror.Items.Count > 100)
                {
                    MainForm.getInstance().shangfanerror.Items.RemoveAt(0);
                }
                else
                {

                }
            MainForm.getInstance().shangfan.BackColor = Color.Red;
            MainForm.getInstance().shangfan.Text = "点击查看警报";
            MainForm.getInstance().shangfanerror.Items.Add("记录" + Str + "温度差是:" + num);
           // MessageBox.Show("记录" + Str + "温度差是:" + num);
        }

        public static float getNewFloat(DataTable a, DataTable b)//获取两个文件对比的最大
        {
            float maxa = 0;

            int Countb = b.Rows.Count;
            for (int i = 0; i < Countb; i++)
            {

                try
                {
                    /*a/b中的fb列不一样，一个是字符串一个是数字*/
                    int bTma = int.Parse(b.Rows[i][1].ToString());//深度、
                    float bTm2b = float.Parse(b.Rows[i][2].ToString());//深度
                    DataRow[] drs3 = a.Select("fb=" + bTma);
                    string ww = drs3[0][2].ToString();
                    float aTma = float.Parse(drs3[0][2].ToString());

                    int bTm = int.Parse(b.Rows[i][1].ToString());//shendu
                    float bTm2 = float.Parse(b.Rows[i][2].ToString());//温度
                    float aTm = float.Parse(a.Rows[bTm][2].ToString());
                    float aTm2 = float.Parse(a.Rows[bTm][1].ToString());
                    float num = bTm2 - aTm;
                    num = System.Math.Abs(num);//取得正数
                    if (num > maxa)
                    {
                        maxa = num;
                    }

                }
                catch
                {

                }
            }
            return maxa;
        }
        //获取基础时间点的数据；
        public static DataTable getJtable(DateTime t)
        {
            MySqlConnection mycon = getMycon();
            System.Data.DataTable dt1=null;
            //判断是否已经导入数据库
            string str = "SELECT DISTINCT folderTime,folderTable from alltemporpary_data WHERE folderTime >=  \'" + t + "\'";
            System.Data.DataTable dt = getDataTable(str, mycon);
            if (dt.Rows.Count != 0)
            {
                string tablename = dt.Rows[0]["folderTable"].ToString();
                string tabletime = dt.Rows[0]["folderTime"].ToString();
                string Str = "SELECT RecordTime,Depth,TM from " + tablename + " WHERE RecordTime = \'" + tabletime + "\' order by Depth";
                dt1 = getDataTable(Str, mycon);
            }
           
            mycon.Close();
            mycon.Dispose();
            return dt1;
            
        }
        
    }
}
