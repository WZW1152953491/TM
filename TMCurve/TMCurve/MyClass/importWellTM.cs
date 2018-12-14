using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TMCurve.MyClass
{
    class importWellTM:MyClass
    {
        public static void importWell()
        {
            try
            {
                CWellTable("wellbore");
            }
            catch { }
           // if (MainForm.getInstance().plateName.Text != "" && MainForm.getInstance().wellName.Text != "" && MainForm.getInstance().textBox21.Text != "")
            if (MainForm.getInstance().textBox21.Text != "")
            {
                try
                {
                   // bool b = Pnum();//判断是否已经存入数据库
                    //if (b == true)//未存入
                    //{
                        //MainForm.getInstance().button11.Enabled = false;
                        DataTable dt = importWellData();
                        usenumber(dt);
                        MessageBox.Show("数据录入成功");

                    //}
                    //else
                    //{
                    //    Thread.Sleep(2000);
                    //    MessageBox.Show("该油井轨迹数据已经录入");
                        
                    //}
                    
                }
                catch
                {
                    
                }
            }
            else
            {
                MessageBox.Show("请将信息填写完整");
            }
            MyClass.goTrue();
        }
        /*创建表格，若是重复，则不再创建*/
        public static void CWellTable(string name)
        { //name应当是一个时间点，或者说是年月日。
            try
            {
                MySqlConnection mycon = new MySqlConnection();
                mycon = getMycon();
                string str = "DROP TABLE IF EXISTS `wellbore`;CREATE TABLE `";
                string str1 = name;
                //string str2 = "` (`ObjectID` int(10) NOT NULL AUTO_INCREMENT ,`Plate` varchar(20)  DEFAULT NULL,`Well` varchar(20)  DEFAULT NULL,`mD` float(10,2) DEFAULT NULL,`inclAngle` float(15,10) DEFAULT NULL,`TVD` float(15,10) DEFAULT NULL,`DepthH` float(15,10) DEFAULT NULL , PRIMARY KEY (`ObjectID`),  INDEX `TIndex` (`Plate`,`Well`,`mD`) USING BTREE  );";
                string str2 = "` (`ObjectID` int(10) NOT NULL AUTO_INCREMENT ,`mD` float(10,2) DEFAULT NULL,`inclAngle` float(15,10) DEFAULT NULL,`TVD` float(15,10) DEFAULT NULL,`DepthH` float(15,10) DEFAULT NULL , PRIMARY KEY (`ObjectID`),  INDEX `TIndex` (`mD`) USING BTREE  );";
                
                string Str = str + str1 + str2;
                doStrmycon(Str, mycon);
                mycon.Close();
                mycon.Dispose();
            }
            catch
            {

            }

        }
        //public static bool Pnum()
        //{
        //    MySqlConnection mycon = new MySqlConnection();
        //    mycon = getMycon();

        //    string Plate = MainForm.getInstance().plateName.Text;
        //    string Well = MainForm.getInstance().wellName.Text;
        //    string se1 = "select * from wellbore where Well='" + Well + "' and Plate ='" + Plate + "'";
        //    // getsqlcom(se1, mycon);

        //    MySqlCommand mycmd = new MySqlCommand(se1, mycon);
        //    object count = mycmd.ExecuteScalar();
        //    if (count != null)
        //    {
        //        mycon.Close();
        //        mycon.Dispose();
        //        return false;
        //    }
        //    else
        //    {
        //        mycon.Close();
        //        mycon.Dispose();
        //        return true;
        //    }
        //}
        /*
       油井轨迹导入
       */

        public static DataTable importWellData()
        {
            DataTable table = new DataTable();//存储有用的数据流
            DataTable table1 = new DataTable();//存储全部数据流
            try
            {
                string foldPath = MainForm.getInstance().textBox21.Text;
                string extension = System.IO.Path.GetExtension(foldPath);//扩展名 “.txt”
                if (extension == ".txt")//判断文件的后缀是否是所需要的
                {
                    table1.Columns.Add("f");
                    using (StreamReader sr1 = new StreamReader(foldPath, Encoding.Default))
                    {
                        while (!sr1.EndOfStream)
                        {
                            DataRow dr = table1.NewRow();//创建数据行
                            string readStr = sr1.ReadLine();//读取一行数据
                            dr["f"] = readStr;
                            table1.Rows.Add(dr);//将创建的数据行添加到table中
                        }
                    }

                    //为数据表创建相对应的数据列
                    table.Columns.Add("fa");
                    table.Columns.Add("fb");
                    table.Columns.Add("fc");
                    for (int i = 5; i < table1.Rows.Count; i++)
                    {
                        DataRow dr = table.NewRow();//创建数据行
                        string readStr = table1.Rows[i][0].ToString();
                        string str1 = readStr.Substring(0, 9).Trim();//测量深度          
                        string str2 = readStr.Substring(10, 9).Trim();//井斜角 
                        string str3 = readStr.Substring(20, 10).Trim();//垂直深度

                        dr["fa"] = str1;
                        dr["fb"] = str2;
                        dr["fc"] = str3;

                        table.Rows.Add(dr);//将创建的数据行添加到table中
                    }
                    DataRow dc = table.NewRow();//创建数据行
                    dc["fa"] = double.Parse(table.Rows[table.Rows.Count - 1]["fa"].ToString()) + 1;
                    dc["fb"] = double.Parse(table.Rows[table.Rows.Count - 1]["fb"].ToString());
                    dc["fc"] = double.Parse(table.Rows[table.Rows.Count - 1]["fc"].ToString());
                    table.Rows.Add(dc);
                }
                else
                {
                   MessageBox.Show("不是.txt文件");
                }
            }
            catch (Exception e)
            {
               MessageBox.Show("出现错误导入未完成" + e);
            }

            return table;
        }


        public static void usenumber(DataTable table)
        {
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();

            string sum = null;
            string str;
            double DepthV = 0;
            double DepthH = 0;
            double aDepthV;
            //string Plate = MainForm.getInstance().plateName.Text;
            //string Well = MainForm.getInstance().wellName.Text;
            string[] objectID = new string[2700];
            for (int j = 0; j < table.Rows.Count - 1; j++)
            {

                DepthV = double.Parse(table.Rows[j][2].ToString());
                float AngleF = float.Parse(table.Rows[j][1].ToString());
                for (int i = (int)double.Parse(table.Rows[j][0].ToString()); i < (int)double.Parse(table.Rows[j + 1][0].ToString()); i++)
                {
                    int Depth = i;
                    float Angle = float.Parse(table.Rows[j][1].ToString());//这个点的Angle
                    float AngleA = float.Parse(table.Rows[j + 1][1].ToString());//x下个点的Angle
                    float subtraction = AngleA - Angle;//减法
                    int denominator = (int)double.Parse(table.Rows[j + 1][0].ToString()) - (int)double.Parse(table.Rows[j][0].ToString());
                    float average = subtraction / denominator;

                    if (i == (int)double.Parse(table.Rows[j][0].ToString()))
                    {

                        DepthH = DepthH + Math.Sin(AngleF * Math.PI / 180);
                    }
                    else
                    {
                        AngleF = AngleF + average;
                        aDepthV = (Math.Cos(average + AngleF * Math.PI / 180));
                        DepthV = DepthV + aDepthV;
                        DepthH = DepthH + Math.Sin(AngleF * Math.PI / 180);

                    }
                   // objectID[i] = Guid.NewGuid().ToString();
                    string stra = str = "('" + Depth + "','" + AngleF + "','" + DepthV + "','" + DepthH + "'),";//有几个字段就写几个，行（i）是不变的，列值累加
                    sum = sum + stra;
                }

            }
            str = "insert into wellbore(mD,inclAngle,TVD,DepthH) values" + sum;
            string str2 = str.Substring(0, str.LastIndexOf(","));//获取SQL语句
            doStrmycon(str2, mycon);
            mycon.Clone();
            mycon.Dispose();
          //  MessageBox.Show("导入完成！");
        }
    }
}
