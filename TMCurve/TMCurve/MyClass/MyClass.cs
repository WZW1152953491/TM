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
using System.Configuration;
//select * from tmrecord where Depth BETWEEN 1 and 4 and RecordTime >'2017-12-29 22:39:52'
//DELETE from tmrecord where 1=1
namespace TMCurve.MyClass
{
    class MyClass
    {

        //连接数据库
        public static MySqlConnection getMycon()
        {
            //string Constr = "server=localhost;port=3306;User Id=root;password=120902;Database=test;";
            string Constr = System.Configuration.ConfigurationManager.ConnectionStrings["strCon"].ToString();
            MySqlConnection mycon = new MySqlConnection(Constr);
            if (mycon.State == ConnectionState.Open)
            {
                mycon.Close();
                MessageBox.Show("上一个数据库连接没有关闭！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            mycon.Open();
            return mycon; 
        }
        //插入删除语句
        public static  void doStrmycon(string SQLstr, MySqlConnection mycon)
        {
            MySqlCommand mycmd = new MySqlCommand(SQLstr, mycon);
            mycmd.ExecuteNonQuery();//插入，更新，删除必备 
        }
        //获取查询出来的数据行数的语句
        public static int getSqlObj(string str, MySqlConnection mycon)
        {
            MySqlCommand mycmd = new MySqlCommand(str, mycon);
            int count = Convert.ToInt32(mycmd.ExecuteScalar());//返回查询的第一行第一列
            return count;
        }
        //删除数据库数据
        public static void delData(string datatable)
        {
            try
            {
                MySqlConnection mycon = new MySqlConnection();
                mycon = getMycon();
                string str = "delete from " + datatable + " where 1=1";
                MySqlCommand mycmd1 = new MySqlCommand(str, mycon);
                mycmd1.ExecuteNonQuery();//插入，更新，删除必备
                mycon.Clone();
                mycon.Dispose();
                MessageBox.Show("删除成功");
            }
            catch
            {
                MessageBox.Show("删除失败");
            }
        
        }
        //执行SQL 获取数据并存入内存表
        public static DataTable getDataTable(string Str, MySqlConnection mycon)
        {
            MySqlDataAdapter sda = new MySqlDataAdapter(Str, mycon);
            System.Data.DataTable dt = new System.Data.DataTable();
            sda.Fill(dt);
            return dt;
        }
        //public static void PatleWell()//为平台井号的列表框添加数据
        //{

        //    mycon = getconn();
        //    mycon.Open();
        //    string str = "select DISTINCT Plate from tmrecord";
        //    System.Data.DataTable dt = getDataTable(str, mycon);
        //    string Plate = dt.Rows[0][0].ToString();
        //    MainForm.getInstance().comboBox2.Text = Plate;
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        MainForm.getInstance().comboBox2.Items.Add(dt.Rows[i][0].ToString());

        //    }
        //    string str1 = "select DISTINCT Well from tmrecord where Plate='" + Plate + "'";
        //    System.Data.DataTable dt1 = getDataTable(str1, mycon);
        //    string Well = dt1.Rows[0][0].ToString();
        //    MainForm.getInstance().comboBox3.Text = Well;
        //    for (int i = 0; i < dt1.Rows.Count; i++)
        //    {
        //        MainForm.getInstance().comboBox3.Items.Add(dt1.Rows[i][0].ToString());

        //    }
           

        //}
        public static void goTrue()
        {
            MainForm.getInstance().groupBoxDTS.Enabled = true;
            MainForm.getInstance().groupBoxGrating.Enabled = true;
            // MainForm.getInstance().groupBoxPre.Enabled = true;
            MainForm.getInstance().groupBoxWell.Enabled = true;
            MainForm.getInstance().groupBoxTMExport.Enabled = true;
            MainForm.getInstance().exportLable.Enabled=false;
            MainForm.getInstance().DTSimportLable.Enabled = false;
            MainForm.getInstance().FBRimportLable.Enabled = false;
            //MainForm.getInstance().groupBoxPreExport.Enabled = true;
        }
       
    }
}
