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
    class importDTS:MyClass
    {
        /*
         在DTS数据导入时，先进行5分钟前最近数据对比（不存在就直接导入），另外每隔20个文件存储一次数据
         */
        public static DataTable gettable()//创建一个新表用于存储路径，时间，名称信息。
        {
            DataTable table = new DataTable();
            table.Columns.Add("fa");
            table.Columns.Add("fb");
            table.Columns.Add("fc");
            return table;
        }
        public static void ImportDts()
        {
            DateTime startTime = DateTime.Now;//计算程序运行时间
            //TimeSpan ts = DateTime.Now - startTime;
            DataTable table =getfenTable();//获取所有tra文档路径信息
            DataTable table1 =getFenTable();//获取不同数据表名称
            ThreadPool.SetMaxThreads(5, 5);//最多执行5个线程
            importDTS t = new importDTS();
            WaitCallback callBack;
            for (int i = 0; i < table1.Rows.Count; i++)
            {
                object ob = new object();
                string tableName = table1.Rows[i][0].ToString();
                //创建数据库数据表
                CreateTable.CDataTable(tableName);
                DataTable newdt = new DataTable();
                newdt = table.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
                DataRow[] rows = table.Select("folderTable='" + tableName + "'"); // 从dt 中查询符合条件的记录； 
                foreach (DataRow row in rows)  // 将查询的结果添加到dt中； 
                {
                    newdt.Rows.Add(row.ItemArray);
                }
                ob = (Object)newdt;
                callBack = new WaitCallback(t.insertData);
                ThreadPool.QueueUserWorkItem(callBack, ob);
            }
            while (true)//判断线程池中的线程是否完全结束
            {
                Thread.Sleep(10000);//这句写着，主要是没必要循环那么多次。去掉也可以。
                int maxWorkerThreads, workerThreads;
                int portThreads;
                ThreadPool.GetMaxThreads(out maxWorkerThreads, out portThreads);
                ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);
                if (maxWorkerThreads - workerThreads == 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    MessageBox.Show("DTS数据导入完成，总共花费时间:" + ts.ToString(), "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MyClass.goTrue();
                    MainForm.getInstance().DTSimportLable.Visible=false;
                    break;
                }
            }
        }
        public static DataTable table = gettable();//实例化表

        /*针对于DTS数据，把路径信息，时间信息，以及要存入的表格信息*/
        public static DataTable getDTSFileName(string foldPath)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        {
            DirectoryInfo TheFolder = new DirectoryInfo(foldPath);
            //遍历文件夹
            try
            {
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    getDTSFileName(NextFolder.FullName);
                }
                //遍历文件
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                {
                    if (NextFile.Extension == ".tra")//判断文件的后缀是否是所需要的
                    {
                        DateTime dt = generalClass.getFileTime(NextFile.FullName);//依靠路径获取时间点
                        DataRow dr = table.NewRow();//创建数据行
                        dr["fa"] = NextFile.FullName;//路径
                        dr["fb"] = dt;//时间
                        dr["fc"] = CreateTable.getTableName(dt, "DTS");//获取文件将要存入的表名称
                        table.Rows.Add(dr);//将创建的数据行添加到table中
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch// (Exception ex)
            {
                //writelog.WriteLog("部分文件夹或文件不可读" + ex);
            }
            return table;
        }

        public static void import_temporpary_data(DataTable table)
        {
            MySqlConnection mycon = MyClass.getMycon();
            try
            {
                string str;
                for (int h = 0; h < table.Rows.Count; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                {
                    string readStr = table.Rows[h][0].ToString();
                    readStr = readStr.Replace("\\", "\\\\");//为了保存路径到数据库，不许进行的操作。
                    DateTime readStr1 =Convert.ToDateTime(table.Rows[h][1].ToString());//时间
                    string readStr2 = table.Rows[h][2].ToString();
                    str = "('" + readStr + "','" + readStr1 + "','" + readStr2 + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
                    string str1 = "insert into temporpary_data(folderUrl,folderTime,folderTable) values" + str;
                    string str2 = str1.Substring(0, str1.LastIndexOf(","));//获取SQL语句
                    MySqlCommand mycmd = new MySqlCommand(str2, mycon);
                    mycmd.ExecuteNonQuery();//插入，更新，删除必备
                }
                mycon.Close();
                mycon.Dispose();
                MessageBox.Show("路径录入成功，点击“导入”!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件路径录入失败!\n" + ex);
            }
          
        }
        public static DataTable getfenTable()//取出路径总表
        {
            DataTable table2 = new DataTable();
            MySqlConnection mycon = MyClass.getMycon();
            string str = "select folderUrl,folderTime,folderTable from temporpary_data" ;
            table2 = MyClass.getDataTable(str, mycon);
            mycon.Close();
            mycon.Dispose();
            return table2;
        }
        public static DataTable getFenTable()//取出路径的分表的名称有哪些
        {
            DataTable table2 = new DataTable();
            MySqlConnection mycon = MyClass.getMycon();
            string str = "select distinct folderTable from temporpary_data";
            table2 = MyClass.getDataTable(str, mycon);
            mycon.Close();
            mycon.Dispose();
            return table2;
        }
        public void insertData(object ob)//分表多线程导入数据的方法
        {
            try
            {
                //连接数据库
                MySqlConnection mycon = getMycon();
                DataTable dt = (DataTable)ob;
                string name = dt.Rows[0][2].ToString();//表名称
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime Time = Convert.ToDateTime(dt.Rows[i][1].ToString());//时间
                    string filename1 = dt.Rows[i][0].ToString();//路径
                    //判断是否已经导入数据库
                   // string StrYN = "select count(*) from  alltemporpary_data where  folderTime='" + Time + "'";//如果等于‘1’下面的代码不执行。
                    DateTime Time1 = Time.AddMinutes(+5);
                    DateTime Time2 = Time.AddMinutes(-5);
                    string StrYN = "select  COUNT(*) from alltemporpary_data where folderTime > '" + Time2 + " ' and folderTime <'" + Time1 + "'";
                    int count = getSqlObj(StrYN, mycon);
                    if (count != 0)
                    {
                        continue;
                    }
                    else
                    {
                        DataTable table1 = MyDataTable.getTraDatatable(filename1);
                        if (table1.Rows.Count != 0)
                        {
                            string str3, sum3 = null;
                            for (int h = 0; h < table1.Rows.Count; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                            {
                                str3 = "('" + Time + "','" + float.Parse(table1.Rows[h][1].ToString()) + "','" + float.Parse(table1.Rows[h][2].ToString()) + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
                                sum3 = sum3 + str3;
                            }
                            str3 = "insert into " + name + "(RecordTime,Depth,TM) values" + sum3;
                            str3 = str3.Substring(0, str3.LastIndexOf(","));//获取SQL语句
                            doStrmycon(str3, mycon);
                            filename1 = filename1.Replace("\\", "\\\\");//为了保存路径到数据库，不许进行的操作。
                            string strA3 = "insert into alltemporpary_data(folderUrl,folderTime,folderTable) values ('" + filename1 + "','" + Time + "','" + name + "')";
                            doStrmycon(strA3, mycon);
                              
                              
                        }
                    }
                      
                }
                mycon.Close();
                mycon.Dispose();
            }
            catch (Exception ex)
            {
               // writelog.WriteLog("" + ex);//说明可能出现的错误
                MessageBox.Show("" + ex);
            }
        }
        //public static string getStr(DataTable table1,DateTime Time,string name)
        //{
        //    string str3, sum3 = null;
        //    for (int h = 0; h < table1.Rows.Count; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
        //    {
        //        str3 = "('" + Time + "','" + int.Parse(table1.Rows[h][1].ToString()) + "','" + float.Parse(table1.Rows[h][2].ToString()) + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
        //        sum3 = sum3 + str3;
        //    }
        //    str3 = "insert into " + name + "(RecordTime,Depth,TM) values" + sum3;
        //    str3 = str3.Substring(0, str3.LastIndexOf(","));//获取SQL语句
        //    return str3; 
            
        //}
        //public  void getSQL(DataTable table, string filename, DateTime Time,string name,MySqlConnection mycon)
        //{
        //    string str, sum = null;
        //    try
        //    {
        //        for (int h = 115; h < 2615; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
        //        {
        //            string readStr = table.Rows[h][1].ToString();
        //            string[] strs = readStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);//将读取的字符串按"制表符/t“和””“分割成数组
        //            str = "('" + Time + "','" + int.Parse(strs[1]) + "','" + float.Parse(strs[2]) + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
        //            sum = sum + str;
        //        }
        //        string str1 = "insert into " + name + "(RecordTime,Depth,TM) values" + sum;
        //        string str2 = str1.Substring(0, str1.LastIndexOf(","));//获取SQL语句
        //        doStrmycon(str2, mycon);
        //        filename = filename.Replace("\\", "\\\\");//为了保存路径到数据库，不许进行的操作。
        //        string str3 = "insert into alltemporpary_data(folderUrl,folderTime,folderTable) values ('" + filename + "','" + Time + "','" + name + "')";
        //        doStrmycon(str3, mycon);
        //    }
        //    catch// (Exception se)
        //    {
        //        // writelog.WriteLog("部分文件夹或文件不可读" + se);
        //    }

        //}
        public static void selectFolder()
        {
            //创建临时表，并将路径信息导入临时表
            try
            {
               CreateTable.CTemporparyTable("temporpary_data");
            }
            catch//(Exception  ex)
            {
                //writelog.WriteLog("a"+ex);
            }
            DataTable table = importDTS.getDTSFileName(MainForm.getInstance().dataFolder.Text);//获取DTS文件的路径，时间，将要存储到的表名称。
            if (table.Rows.Count != 0)
            {
                importDTS.import_temporpary_data(table);//存入数据库
            }
            else
            {
                MessageBox.Show("该路径中没有DTS文档");
            }
            MyClass.goTrue();
            MainForm.getInstance().DTSimportLable.Visible = false;
        } 
    }
}
