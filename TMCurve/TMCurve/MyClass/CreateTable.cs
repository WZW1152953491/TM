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
    class CreateTable:MyClass
    {
        //CREATE TABLE \"temporpary_data\" (\"folderTime\" int(10) DEFAULT NULL,\"folderUrl\" varchar(100) COLLATE utf8_bin DEFAULT NULL,\"folderUse\" int(10) DEFAULT NULL);";
        
        public static void CDataTable(string name )
        { //name应当是一个时间点，或者说是年月日。
            try
            {
                MySqlConnection mycon = new MySqlConnection();
                mycon = getMycon();
                string str = "CREATE TABLE `";
                string str1 = name;
                string str2 = "` (`ObjectID`  int(10) NOT NULL AUTO_INCREMENT ,`RecordTime`  datetime NULL DEFAULT NULL ,`Depth`  float(8,3) NULL DEFAULT NULL ,`TM`  float(15,10) NULL DEFAULT NULL ,PRIMARY KEY (`ObjectID`),INDEX `TimeIndex` (`RecordTime`, `Depth`, `TM`)USING BTREE  );";
                string Str = str + str1 + str2;
                doStrmycon(Str, mycon);
                mycon.Close();
                mycon.Dispose();
            }
            catch //(Exception ex)
            {
            //已经创建了表，就不在创建
            }

           
        }
        public static void CreateDatabase()
        {
            string Constr = "server=localhost;port=3306;User Id=root;password=120902;";
            MySqlConnection mycon = new MySqlConnection(Constr);
            if (mycon.State == ConnectionState.Open)
            {
                mycon.Close();
                MessageBox.Show("上一个数据库连接没有关闭！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            mycon.Open();
            //return mycon; 
           // MySqlConnection mycon = new MySqlConnection();
           // mycon = getMycon();
            string str = "Create database test;";
            doStrmycon(str, mycon);
            mycon.Close();
            mycon.Dispose();
        }
        public static void CTemporparyTable(string name)
        {
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            string str;
            str = "DROP TABLE IF EXISTS `temporpary_data`;CREATE TABLE `";//加入创建的是临时数据表，那么每次创建都要删除重建一次，另外要切记，此表运行时，其他功能不能重复运行
            string str1 = name;
            string  str2 = "` (`ObjectID` int(10) NOT NULL AUTO_INCREMENT, `folderUrl` varchar(250) COLLATE utf8_bin DEFAULT NULL,`folderTime` datetime DEFAULT NULL, `folderTable` varchar(30) COLLATE utf8_bin DEFAULT NULL, PRIMARY KEY (`ObjectID`));";
            string Str = str + str1 + str2;
            doStrmycon(Str, mycon);
            mycon.Close();
            mycon.Dispose();
        }
        public static void CAllTemporparyTable(string name)
        {
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            string str;
            str = "CREATE TABLE `";
            string str1 = name;
            //string str2 = "\" (\"ObjectID\" int(10) NOT NULL AUTO_INCREMENT,\"folderTime\" datetime DEFAULT NULL, \"Url\"  int(2) DEFAULT NULL, PRIMARY KEY (\"ObjectID\"));";
            string str2 = "` (`ObjectID` int(10) NOT NULL AUTO_INCREMENT, `folderUrl` varchar(250) COLLATE utf8_bin DEFAULT NULL,`folderTime` datetime DEFAULT NULL, `folderTable` varchar(30) COLLATE utf8_bin DEFAULT NULL, PRIMARY KEY (`ObjectID`));";   
            string Str = str + str1 + str2;
            doStrmycon(Str, mycon);
            mycon.Close();
            mycon.Dispose();
        }
        //public static void gaibian()
        //{
        //   // alter table rsinfo alter column djh varchar(10)
        //    MySqlConnection mycon = new MySqlConnection();
        //    mycon = getMycon();
        //   // alter table student modify column sname varchar(20);
        //
        //    doStrmycon(Str, mycon);
        //    mycon.Close();
        //    mycon.Dispose();
        //}
        public static string getGratTableName(DateTime time,string datasource)//依靠时间获取datasource 就是DTS，压力，光纤温度三种名称。
        {
            string name = time.ToString("yyyyMM");//获取年月
            int day = Convert.ToInt32(time.ToString("dd"));//获取天数
            string xun="月表";
            //if (day <= 10)
            //{
            //    xun = "上旬";
            //}
            //else if (day <= 20 && day > 10)
            //{
            //    xun = "中旬";
            //}
            //else
            //{
            //    xun = "下旬";
            //}
            //string Plate = "ld27_2";//平台与井的名称，可以进行修改。
            //string Well = "a22h";
            //string Name = Plate + "_" + Well + "_"+datasource+name+xun;
            string Name =  datasource + name + xun;
            return Name;
        }
        public static string getTableName(DateTime time, string datasource)//依靠时间获取datasource 就是DTS，压力，光纤温度三种名称。
        {
            string name = time.ToString("yyyyMM");//获取年月
            //string Plate = "ld27_2";//平台与井的名称，可以进行修改。
            //string Well = "a22h";
            //string Name = Plate + "_" + Well + "_" + datasource + name + xun;
            int day = Convert.ToInt32(time.ToString("dd"));//获取天数
            string xun;
            if (day <= 10)
            {
                xun = "上旬";
            }
            else if (day <= 20 && day > 10)
            {
                xun = "中旬";
            }
            else
            {
                xun = "下旬";
            }
           
            string Name =  datasource + name + xun;
            return Name;
        }
        public static void createDTSData(string Name)
        {
            
        }
        public static void Depcom()//为控件添加数据。光栅温度的全部深度
        {
            DataTable table = new DataTable();
            table.Columns.Add("fa");
            table.Columns.Add("fb");
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            string str = "select folderUrl from allgrat_data limit 1;";
            DataTable dt = MyClass.getDataTable(str, mycon);
            mycon.Close();
            mycon.Dispose();
            string filename1 = dt.Rows[0][0].ToString();
            try
            {
                using (StreamReader sr = new StreamReader(filename1, Encoding.Default))
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
                for (int h = 1; h < 21; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                {
                    string readStr = table.Rows[h][1].ToString();
                    string[] strs = readStr.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//将读取的字符串按"制表符/t“和””“分割成数组
                    int aa = int.Parse(strs[1]);
                    // sum = sum + aa+",";
                    MainForm.getInstance().Depcom.Items.Add(aa);
                }
            }
            catch
            {

            }
          
             // string  sum = null;

           
          
        }

    }
}
