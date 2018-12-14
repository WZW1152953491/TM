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
using Microsoft.Office.Interop.Excel;
/*利用getstr1方法的到4种情况下的SQL语句，使用work()方法来利用SQL语句得到临时表，然后导出到Execl 2018.2.26完成*/

namespace TMCurve.MyClass
{
    class export:MyClass
    {
        //public static MySqlConnection mycon;
        public static String singleDepth;//单项深度
        public static float intervalDepth1;//区间深度1
        public static float intervalDepth2;//区间深度2
        public static float intervalTM1;//区间温度1
        public static float intervalTM2;//区间温度2
        public static DateTime intervalTime1;//区间时间1
        public static DateTime intervalTime2;//区间时间2
       

        public static void exportData()//导出
        { 
            int exportNum=0;
            try
            {
                MySqlConnection mycon = new MySqlConnection();
                mycon = getMycon();
                DateTime startTime = DateTime.Now;//计算程序运行时间
                string str1 = getstr1();
                System.Data.DataTable tableName;
                if (MainForm.getInstance().TMname.Text == "DTS温度解调仪")
                {
                    tableName = drawingDTSDep.getTNameTable(intervalTime1, intervalTime2);
                     exportNum=1;
                }
                else
                {
                    tableName = drawingGDep.getTNameTable(intervalTime1, intervalTime2);//获取需要使用的表名称

                }
                if (str1 != null)
                {
                    //string str3 = null;
                    //string str3num = null;
                    //System.Data.DataTable dtValue = new System.Data.DataTable();
                    //for (int i = 0; i < tableName.Rows.Count; i++)
                    //{
                    //    str3 = "SELECT Depth,RecordTime,TM from  " + tableName.Rows[i][0] + " " + str1;
                    //    str3num = str3num + str3 + " union ";
                    //}

                    string str3 = null;
                   // string str3num = null;
                    System.Data.DataTable dtValue = new System.Data.DataTable();
                    System.Data.DataTable dt = new System.Data.DataTable();
                    for (int i = 0; i < tableName.Rows.Count; i++)
                    {
                        dt = null;
                        str3 = null;
                        str3 = "SELECT Depth,RecordTime,TM from  " + tableName.Rows[i][0] + " " + str1 + " ORDER BY RecordTime";
                        dt = getDataTable(str3, mycon);
                        dtValue.Merge(dt);
                    }
                   
                   // str3num = str3num.Remove(str3num.Length - 6, 6) + " ORDER BY RecordTime, Depth";
                    //数据存入临时表
         

                    //获取文件名称
                  //  string saveFileName ;
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.DefaultExt = "csv";
                    saveDialog.Filter = "Excel文件|*.csv";

                    saveDialog.Title = "请填写文件名和选择文件路径";
                    saveDialog.FileName = "新建";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        //saveFileName = saveDialog.FileName;
                        try
                        {
                            SaveCSV(dtValue, saveDialog.FileName, exportNum);
                            TimeSpan ts = DateTime.Now - startTime;
                            MessageBox.Show("文件： " + saveDialog.FileName + "保存成功，总共花费时间:" + ts.ToString(), "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            mycon.Close();
                        }
                        catch (Exception e)
                        {
                            MyClass.goTrue();
                            MessageBox.Show("导出失败" + e);
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                MyClass.goTrue();
                MessageBox.Show("导出失败" + ex);
            }
            MyClass.goTrue();
         //   MainForm.threadImport.Abort();
        }

        public static string  getstr1()//获取SQL语句
        {
            string str1=null;
            float wellzero = ZedGraphClass.getWellZero();
            if (MainForm.getInstance().intervalTime2.Value >= MainForm.getInstance().intervalTime1.Value)//如果时间正确
            {
                intervalTime1 = MainForm.getInstance().intervalTime1.Value;
                intervalTime2 = MainForm.getInstance().intervalTime2.Value;
                if (MainForm.getInstance().singleDepth.Text == "")//如果深度单项为空
                {
                    try
                    {
                        intervalDepth1 = float.Parse(MainForm.getInstance().intervalDepth1.Text) + wellzero;
                        intervalDepth2 = float.Parse(MainForm.getInstance().intervalDepth2.Text) + wellzero;
                        if (intervalDepth2 >= intervalDepth1 && intervalDepth1>= 0)
                        {
                            if (MainForm.getInstance().intervalTM1.Text == "" && MainForm.getInstance().intervalTM2.Text == "")//如果温度为空
                            {
                                str1 = "  WHERE RecordTime BETWEEN  \'" + intervalTime1 + "\' and \'" + intervalTime2 + "\'and Depth between " + intervalDepth1 + " and " + intervalDepth2 ;
                              
                            }
                            else if (MainForm.getInstance().intervalTM1.Text != "" && MainForm.getInstance().intervalTM2.Text != "")//如果温度存在
                            {
                                intervalTM1 = float.Parse(MainForm.getInstance().intervalTM1.Text);
                                intervalTM2 = float.Parse(MainForm.getInstance().intervalTM2.Text);
                                if (intervalTM2 >= intervalTM1)//
                                {

                                    str1 = "   WHERE RecordTime BETWEEN  \'" + intervalTime1 + "\' and \'" + intervalTime2 + "\'and Depth between " + intervalDepth1 + " and " + intervalDepth2 + " and TM between  " + intervalTM1 + " and " + intervalTM2 ;
                                    string a = str1;
                                    //MessageBox.Show("热“");
                                }
                                else { MessageBox.Show("区间温度填写不正确", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                            }
                            else { MessageBox.Show("请将区间温度填写完成", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }

                        }
                        else { MessageBox.Show("深度区间不正确", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    }
                    catch 
                    {
                       
                        MessageBox.Show("区间深度输入格式不正确\n", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }
                else{//如果深度单项为条件
                   try
                   {
                       singleDepth = MainForm.getInstance().singleDepth.Text;
                       String [] strs = singleDepth.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                       int[] strs1 = new int[strs.Length];
                       for (int i = 0; i < strs.Length; i++)//将单项深度导入int数组，看是否正确
                       {
                          
                           strs1[i] = int.Parse(strs[i]);
                       }
                       string singleDepth1=null;
                       for(int i=0;i<strs.Length;i++){
                           float dep = strs1[i] + wellzero;
                           singleDepth1=singleDepth1+dep+',';
                       }
                       singleDepth1 = singleDepth1.Remove(singleDepth1.Length - 1,1) ;
                       if (MainForm.getInstance().intervalTM1.Text == "" && MainForm.getInstance().intervalTM2.Text == "")//如果温度为空
                           {
                               str1 = " WHERE RecordTime BETWEEN  \'" + intervalTime1 + "\' and \'" + intervalTime2 + "\'and Depth in (" + singleDepth1 + ") ";
                           }
                       else if (MainForm.getInstance().intervalTM1.Text != "" && MainForm.getInstance().intervalTM2.Text != "")//如果温度存在
                       {
                           intervalTM1 = float.Parse(MainForm.getInstance().intervalTM1.Text);
                           intervalTM2 = float.Parse(MainForm.getInstance().intervalTM2.Text);
                           if (intervalTM2 >= intervalTM1)//
                           {

                               str1 = "  WHERE RecordTime BETWEEN  \'" + intervalTime1 + "\' and \'" + intervalTime2 + "\' and Depth in (" + singleDepth1 + ") and TM between  " + intervalTM1 + " and " + intervalTM2 ;
                               string a = str1;
                           }
                           else { MessageBox.Show("区间温度填写不正确", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                       }
                       else { MessageBox.Show("请将区间温度填写完成", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                       
                   }
                   catch //深度格式不正确
                   {
                       
                       MessageBox.Show("单项深度输入格式不正确\n", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   }
                }
            }
            else {
                MessageBox.Show("时间选择不正确", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return str1;
        }
        public static void SaveCSV(System.Data.DataTable dt, string fullPath, int exportNum)//table数据写入csv  
        {
            //exportNum：最初DTS和FBG不一样，只有DTS需要井口位置，现在都需要了，所以这个量不需要了，但是现在指示代码注释。
            float wellzero = ZedGraphClass.getWellZero();
            System.IO.FileInfo fi = new System.IO.FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Create,
                System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "列名,";
            string data1;
            System.Data.DataTable dtTime = null;//获取时间
            System.Data.DataTable dtDepth = null;//获取深度
            if (MainForm.getInstance().comboBox1.Text == "深度为纵轴，时间为横轴")
            {
                dtTime = export.SelectDistinct(dt, dt.Columns[1].ColumnName.ToString());//获取不同的时间数据
                dtDepth = export.SelectDistinct(dt, dt.Columns[0].ColumnName.ToString());//获取不同的深度数据
                for (int i = 0; i < dtTime.Rows.Count; i++)//写入列名  
                {
                    data1 = dtTime.Rows[i][0].ToString();
                    data += data1;
                    if (i < dtTime.Rows.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                for (int i = 0; i < dtDepth.Rows.Count; i++) //写入各行数据  
                {
                    //得到row的数据
                    string data2;
                    //if (exportNum!=1)
                    //{
                    //     data2 = dtDepth.Rows[i][0].ToString();
                    //}
                    //else{
                         data2 = ((float)(dtDepth.Rows[i][0]) - wellzero).ToString();
                    //}
                    string aaa = dtDepth.Rows[i][0].ToString();
                    DataRow[] drss = null;
                    drss = dt.Select("Depth = '" + aaa + "'","RecordTime asc");//每行数据
                    for (int k = 0; k < drss.Length; k++)
                    {
                        string b = drss[k][2].ToString();
                        data2 += "," + b;
                    }
                    sw.WriteLine(data2);
                }
            }
            else
            {
                dtTime = export.SelectDistinct(dt, dt.Columns[0].ColumnName.ToString());
                dtDepth = export.SelectDistinct(dt, dt.Columns[1].ColumnName.ToString());
                for (int i = 0; i < dtTime.Rows.Count; i++)//写入列名  
                {

                    //if (exportNum != 1)
                    //{
                    //    data1 = dtTime.Rows[i][0].ToString();
                    //}
                    //else
                    //{
                    data1 = ((float)(dtTime.Rows[i][0]) - wellzero).ToString();
                    //}
                    data +=data1;//"\t"是为了导出时保留原格式
                    if (i < dtTime.Rows.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                for (int i = 0; i < dtDepth.Rows.Count; i++) //写入各行数据  
                {
                    //得到row的数据
                    string data2 = dtDepth.Rows[i][0].ToString();
                   // data2 = "\t" + data2;
                   // data2 = data2;
                    DataRow[] drss = null;
                    drss = dt.Select("RecordTime = '" + data2 + "'", "Depth asc");//从小到大
                    for (int k = 0; k < drss.Length; k++)
                    {
                        string b = drss[k][2].ToString();
                       // data2 += "," + "\t" + b;
                        data2 += ","+ b;
                    }
                    sw.WriteLine(data2);
                }

            }

          
            sw.Close();
            fs.Close();

        }
        public static  System.Data.DataTable SelectDistinct(System.Data.DataTable dt,string lname)
        {
            DataView dataview= dt.DefaultView;
            System.Data.DataTable ds=dataview.ToTable(true,lname);
            return ds;
        }
    }
}
