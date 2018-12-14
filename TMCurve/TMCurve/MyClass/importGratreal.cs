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
    class importGratreal:MyClass
    {
        public static DataTable getGrattable()//创建一个新表用于存储路径，时间，名称信息。画图所用的表
        {
            DataTable table = new DataTable();
            table.Columns.Add("fa");
            //table.Columns.Add("fb");
            //table.Columns.Add("fc");
            return table;
        }
        public static DataTable Grattable = getGrattable();
        public  static List<DataTable> Gratlist = new List<DataTable>();
        public static int counGrat;
        public static void jiankongGrat()
        {
            var g_Watcher = new System.IO.FileSystemWatcher();
            g_Watcher.Path = MainForm.getInstance().realGrat1.Text;
            g_Watcher.Filter = "*.fbr";
            g_Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            g_Watcher.Created += g_Watcher_Changed;//复制时出现了一次。
            g_Watcher.IncludeSubdirectories = true;
            g_Watcher.EnableRaisingEvents = true;
            //m_Watcher.Changed += m_Watcher_Changed;//复制时会出现两次，估计是开始创建到创建完成 
            //m_Watcher.Deleted += m_Watcher_Changed;//删除时出现了
            //m_Watcher.Renamed += m_Watcher_Changed;
        }

        private static void g_Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                //int Linenumber = Convert.ToInt32(MainForm.getInstance().GratLineNumber.Text);
                string NewFile = moveFile(e.FullPath, MainForm.getInstance().realGrat2.Text);//文件移动并得到新的路径
                getDrawTable(NewFile);//存数据到list
                if (Gratlist.Count <= 10) {}
                else
                {
                    Gratlist.RemoveAt(0);
                    Grattable.Rows[0].Delete();
                    counGrat++;
                    if (counGrat > 9)
                    {
                        counGrat = 0;
                        string name = Grattable.Rows[0][0].ToString();
                        importGratReal(Gratlist[0], name);//需要去重复
                    }          
                }
                if (Gratlist.Count != 0)
                {
                    importRealGrat(Gratlist);
                    drawingRealGrat.drawRealGrat(Gratlist);
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("画图失败，请查看是否填写正确！" + ex);
               // writelog.WriteLog("" + ex);
            }

        }
        public static void importRealGrat(List<DataTable> dt)//依靠内存list存入数据库，先创建表
        {
            int du = 9;
            int du1 = 5;
            if (MainForm.getInstance().realGrat3.Text != "")
            {
                du = Convert.ToInt32(MainForm.getInstance().realGrat3.Text);//对比的时间间隔
            }
            if (MainForm.getInstance().realGrat4.Text != "")
            {
               du1 = Convert.ToInt32(MainForm.getInstance().realGrat4.Text);//对比的数值；
            }
          
            if (dt.Count <= 9)
            {

                //存入数据库（dt[dt.Count-1]）前10个文件全部存入
                importGratReal(dt[dt.Count - 1], Grattable.Rows[dt.Count - 1][0].ToString());
            }
            else
            {
                DataTable table1 = dt[dt.Count - 1];
                string name = Grattable.Rows[dt.Count - 1][0].ToString();
                DataTable table2 = dt[dt.Count - 1 - du];
                float max = getFloat(table1, table2);
                if (max > du1)
                {
                    //存入数据库（table1）
                    importGratReal(table1, name);
                }
                else
                {
                    //不是特殊数据不存入
                }
            }

        }
        private static float getFloat(DataTable a, DataTable b)//获取两个文件对比的最大
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
            MainForm.getInstance().FBGdifMax.Text = maxa.ToString();
            return maxa;

        }
        /*文件移动，另创建特定的、以天为分割的文件夹*/
        public static string moveFile(string OrignFile, string NewFile)
        {
            string timestr = OrignFile.Substring(OrignFile.LastIndexOf("\\") + 1, OrignFile.LastIndexOf(".") - (OrignFile.LastIndexOf("\\") + 1));//去除后缀
            string str = timestr.Remove(0, timestr.Length - 14);//取去除后缀的文件名称的后14位数据。
            str = str.Remove(8);
            NewFile = NewFile + "\\" + str;
            if (!Directory.Exists(NewFile))//判断文件夹是否存在，不存在则创建
            {
                Directory.CreateDirectory(NewFile);
            }
            NewFile = NewFile + "\\" + timestr + ".fbr";
            try
            {
                File.Move(OrignFile, NewFile);
            }
            catch { }
            return NewFile;
        }
        /*向表中添加路径信息*/
        private static void getDrawTable(string filename)
        {
            DataTable ob = getDTSRealData(filename);//存数据到内存表
            if (ob.Rows.Count == 0)//如果是空的table则不存入列表
               
            {

                MainForm.getInstance().errorFBR.Text = "警告：最新时刻的文档不正确（乱码）！";
            }
            else
            {

                MainForm.getInstance().errorFBR.Text = "";
                Gratlist.Add(ob);
                DataRow dr = Grattable.NewRow();//创建数据行
                dr["fa"] = filename;
                Grattable.Rows.Add(dr);//将创建的数据行添加到table中
              
            }
            
        }
        public static DataTable getDTSRealData(string filename)
        {
            string timestr = filename.Substring(filename.LastIndexOf("\\") + 1, filename.LastIndexOf(".") - (filename.LastIndexOf("\\") + 1));//去除后缀
            string str = timestr.Remove(0, timestr.Length - 14);//取去除后缀的文件名称的后14位数据。
            DateTime Time = DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
      
            using (DataTable table = new DataTable())//深度、温度
            {
                //为数据表创建相对应的数据列
                table.Columns.Add("fa");//时间
                table.Columns.Add("fb");//深度
                table.Columns.Add("fc");//温度
                using (StreamReader sr = new StreamReader(filename, Encoding.Default))//读取每一行数据到临时表
                {
                    int k = 0;
                    while (!sr.EndOfStream)
                    {
                        string readStr = sr.ReadLine();//读取一行数据
                        DataRow dr = table.NewRow();//创建数据行
                        dr["fa"] = k + 1;
                        dr["fb"] = readStr;
                        table.Rows.Add(dr);//将创建的数据行添加到table中
                    }
                }

                DataTable table1 = new DataTable();
                
                    table1.Columns.Add("fa");//时间
                    table1.Columns.Add("fb");//深度/
                    table1.Columns.Add("fc");//温度

                    try
                    {
                        for (int h = 1; h < 21; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                        {
                            string readStr = table.Rows[h][1].ToString();
                            string[] strs = readStr.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//将读取的字符串按"制表符/t“和””“分割成数组
                            DataRow dr = table1.NewRow();//创建数据行
                            dr["fa"] = Time;
                            dr["fb"] = int.Parse(strs[1]);
                            dr["fc"] = float.Parse(strs[2]);
                            table1.Rows.Add(dr);//将创建的数据行添加到table中
                        }
                    }
                    catch //(Exception se)
                    {
                       // table1 = null;
                       // writelog.WriteLog("部分文件夹或文件不可读" + se + filename);
                    }
                    return table1;


                
            }

        }
        public static void importGratReal(DataTable table, string filename)
        {

             MySqlConnection mycon = getMycon();
             DateTime dt = Convert.ToDateTime(table.Rows[0][0]);
           
            //判断是否已经导入数据库
             string Str = "select folderTime from  allGrat_data where  folderTime='" + dt + "'";//如果等于‘1’下面的代码不执行。
            MySqlCommand mycmd = new MySqlCommand(Str, mycon);
            object count = mycmd.ExecuteScalar();
            if (count != null)
            {

            }
            else
            {
                //DateTime dt = Convert.ToDateTime(table.Rows[0][0]);
                //  = DateTime.ParseExact(Str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);  
                string name = CreateTable.getGratTableName(dt, "fbr");//获取文件将要存入的表名称
                CreateTable.CDataTable(name);//创建表
                string str, sum = null;
                try
                {
                    for (int h = 0; h < table.Rows.Count; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                    {
                        str = "('" + dt + "','" + int.Parse(table.Rows[h][1].ToString()) + "','" + float.Parse(table.Rows[h][2].ToString()) + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
                        sum = sum + str;
                    }
                    string str1 = "insert into " + name + "(RecordTime,Depth,TM) values" + sum;
                    string str2 = str1.Substring(0, str1.LastIndexOf(","));//获取SQL语句
                    doStrmycon(str2, mycon);
                    filename = filename.Replace("\\", "\\\\");//为了保存路径到数据库，不许进行的操作。
                    string str3 = "insert into allgrat_data(folderUrl,folderTime,folderTable) values ('" + filename + "','" + dt + "','" + name + "')";
                    doStrmycon(str3, mycon);
                }
                catch //(Exception se)
                {
                   // writelog.WriteLog("部分文件夹或文件不可读" + se);
                }
                
            }
            mycon.Close();
            mycon.Dispose();

        }
    }
}
