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
    class importGrat : MyClass
    {

        public static void ImportGrat()
        {
            DateTime startTime = DateTime.Now;//计算程序运行时间
            DataTable table =importDTS.getfenTable();
            DataTable table1 = importDTS.getFenTable();//获取数据路径，时间，表名称
            ThreadPool.SetMaxThreads(5, 5);//最多执行5个线程
            importGrat t = new importGrat();
            WaitCallback callBack;
            for (int i = 0; i < table1.Rows.Count; i++)
            {
                string tableName = table1.Rows[i][0].ToString();
                CreateTable.CDataTable(tableName);
                DataTable newdt = new DataTable();
                newdt = table.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
                DataRow[] rows = table.Select("folderTable='" + tableName + "'"); // 从dt 中查询符合条件的记录； 
                foreach (DataRow row in rows)  // 将查询的结果添加到dt中； 
                {
                    newdt.Rows.Add(row.ItemArray);
                }
                object ob = new object();
                ob = (Object)newdt;
                callBack = new WaitCallback(t.insertGratData);
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
                   // MessageBox.Show("导入完成！");
                    TimeSpan ts = DateTime.Now - startTime;
                    MessageBox.Show("数据导入完成，总共花费时间:" + ts.ToString(), "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MyClass.goTrue();
                    MainForm.getInstance().FBRimportLable.Visible = false;
                    break;
                }
            }
        }
        public static DataTable table = importDTS.gettable();//实例化表

        /*针对于Grat数据，把路径信息，时间信息，以及要存入的表格信息*/
        public static DataTable getfileGrat(string foldPath)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(foldPath);
            //遍历文件夹
            try
            {
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    getfileGrat(NextFolder.FullName);
                }
                //遍历文件
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                {
                    if (NextFile.Extension == ".fbr")//判断文件的后缀是否是所需要的
                    {
                        string filename = NextFile.FullName;
                        string timestr = filename.Substring(filename.LastIndexOf("\\") + 1, filename.LastIndexOf(".") - (filename.LastIndexOf("\\") + 1));//去除后缀
                        string str = timestr.Remove(0, timestr.Length - 14);//取去除后缀的文件名称的后14位数据。
                        DateTime dt = DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                       
                        DataRow dr = table.NewRow();//创建数据行
                        dr["fa"] = NextFile.FullName;//路径
                        dr["fb"] = dt;//时间
                        dr["fc"] = CreateTable.getGratTableName(dt, "fbr");//获取文件将要存入的表名称
                        table.Rows.Add(dr);//将创建的数据行添加到table中
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch //(Exception ex)
            {
                //writelog.WriteLog("部分文件夹或文件不可读" + ex);
            }

            return table;

        }

        public void insertGratData(object ob)//分表多线程导入数据的方法
        {
            //连接数据库

            try
            {
                MySqlConnection mycon = getMycon();
                DataTable dt = (DataTable)ob;
                string name = dt.Rows[0][2].ToString();//表名称
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime Time = (DateTime)dt.Rows[i][1];//时间
                    //判断是否已经导入数据库
                    string Str = "select count(*) from " + name + " where  RecordTime='" + Time + "'";//如果等于‘1’下面的代码不执行。
                    MySqlCommand mycmd = new MySqlCommand(Str, mycon);
                   // object count = mycmd.ExecuteScalar();
                    int count = getSqlObj(Str, mycon);
                    if (count != 0)
                    {
                        continue;
                    }
                    else
                    {
                        string filename = dt.Rows[i][0].ToString();//路径
                        using (DataTable table = new DataTable())//深度、温度
                        {
                            //为数据表创建相对应的数据列
                            table.Columns.Add("fa");
                            table.Columns.Add("fb");
                            using (StreamReader sr = new StreamReader(filename, Encoding.Default))
                            {
                                int k = 0;
                                while (!sr.EndOfStream)
                                {
                                    DataRow dr = table.NewRow();//创建数据行
                                    string readStr = sr.ReadLine();//读取一行数据
                                    dr["fa"] = k + 1;
                                    dr["fb"] = readStr;
                                    table.Rows.Add(dr);//将创建的数据行添加到table中
                                }
                            }

                            //拼接insert语句
                            string str, sum = null;
                            try
                            {
                                for (int h = 1; h < 21; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                                {
                                    string readStr = table.Rows[h][1].ToString();
                                    string[] strs = readStr.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);//将读取的字符串按"制表符/t“和””“分割成数组
                                    int aa = int.Parse(strs[1]);
                                    str = "('" + Time + "','" + int.Parse(strs[1]) + "','" + float.Parse(strs[2]) + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
                                    sum = sum + str;
                                }
                                string str1 = "insert into " + name + "(RecordTime,Depth,TM) values" + sum;
                                string str2 = str1.Substring(0, str1.LastIndexOf(","));//获取SQL语句
                                doStrmycon(str2, mycon);
                                filename = filename.Replace("\\", "\\\\");//为了保存路径到数据库，不许进行的操作。
                                string str3 = "insert into allgrat_data(folderUrl,folderTime,folderTable) values ('" + filename + "','" + Time + "','" + name + "')";
                                doStrmycon(str3, mycon);
                            }
                            catch //(Exception se)
                            {
                                //writelog.WriteLog("部分文件夹或文件不可读" + se);
                            }
                        }
                    }

                }
                mycon.Close();
                mycon.Dispose();

            }
            catch//(Exception ex)
            {
              // writelog.WriteLog("部分文件夹或文件不可读" + ex);
            }

        }
        public static void selectFolderGrat()
        {
            try
            {
                CreateTable.CTemporparyTable("temporpary_data");
            }
            catch
            {

            }
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";
            DataTable table = importGrat.getfileGrat(MainForm.getInstance().GratFolder.Text);//获取DTS文件的路径，时间，将要存储到的表名称。
            if (table.Rows.Count != 0)
            {
                importDTS.import_temporpary_data(table);//存入数据库
            }
            else
            {
                MessageBox.Show("该路径中没有FBG文档");
            }
            MyClass.goTrue();
            MainForm.getInstance().FBRimportLable.Visible = false;
        }
    }
}
