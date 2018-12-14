using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql;
using MySql.Data.MySqlClient;
using System.Collections;
using System.IO;

namespace TMCurve.MyClass
{
    class MyDataTable : MyClass
    {
       /*
        * 获取一个表A的某一行，加入另一个表B的方法：先个B新建行，读取A的数据，加入B新建行，添加
        */
        public DataTable gettable(int number)//创建一个新表用于存储路径，时间，名称信息。
        {
            DataTable table = new DataTable();
            for (int i = 0; i < number; i++)
            {
                table.Columns.Add(i.ToString());
            }
            return table;
        }
        public static void  getColon(DataTable table ,string tableName)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                object ob = new object();
                DataTable newdt = new DataTable();
                newdt = table.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
                DataRow[] rows = table.Select("folderTable='" + tableName + "'"); // 从dt 中查询符合条件的记录； 
                foreach (DataRow row in rows)  // 将查询的结果添加到dt中； 
                {
                    newdt.Rows.Add(row.ItemArray);
                }
                ob = (Object)newdt;
            }
        }
        public static DataTable getDataTable(DataTable SQLName, int h, string SQLstr, string SQLque, MySqlConnection mycon)
        {
         
            string Str = null;
            DataTable dtValue = new DataTable();
            System.Data.DataTable dt = new DataTable();
            for (int j = 0; j < SQLName.Rows.Count; j++)
            {
                Str = null;
                Str = "SELECT Depth,RecordTime,TM from " + SQLName.Rows[j][0] + " " + SQLstr + " and Depth=" + h + " ORDER BY " + SQLque;
                dt = getDataTable(Str, mycon);
                dtValue.Merge(dt);

            }
            return dtValue;
        }
        public static DataTable getDistinckTable(DataTable  dtTimeTable)
        {
          DataTable distinckTable = dtTimeTable.Clone();
          DataView dv = new DataView(dtTimeTable); //虚拟视图吧，我这么认为
          string[] aaa = GetTableColumnName(dtTimeTable);
          distinckTable = dv.ToTable(true, aaa);
          return distinckTable;

        }
        public static string[] GetTableColumnName(DataTable dt)
        {
            string cols = string.Empty;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                cols += (dt.Columns[i].ColumnName + ",");
            }
            cols = cols.TrimEnd(',');
            return cols.Split(',');
        }
        public static ArrayList getDepth(float wellzero, string strDepth)//得到曲线的深度集合
        {

            ArrayList ADepth = new ArrayList();
            //  DsingleDepth = MainForm.getInstance().DsingleDepth.Text;
            String[] strs = strDepth.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strs.Length; i++)//将单项深度导入int数组，看是否正确
            {
                ADepth.Add(float.Parse(strs[i]) + wellzero);
            }
            return ADepth;
        }
        /*//数据表查询行以及各式复制
          newdt = table.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
                       DataRow[] rows = table.Select("folderTable='" + tableName + "'"); // 从dt 中查询符合条件的记录； 
                       foreach (DataRow row in rows)  // 将查询的结果添加到dt中； 
                       {
                           newdt.Rows.Add(row.ItemArray);
                       }
         */
        public static DataTable getTraDatatable(string filename)
        {
            int strNum = drawAttribute.getStrNum();
            DateTime Time = generalClass.getFileTime(filename);
            using (DataTable table = new DataTable())
            {
                //为数据表创建相对应的数据列
                table.Columns.Add("fa");
                table.Columns.Add("fb");
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

                DataTable table1 = new DataTable();//深度、温度

                table1.Columns.Add("Time");//时间
                table1.Columns.Add("Depth");//深度
                table1.Columns.Add("TM");//温度

                try
                {
                    int k = strNum + 115;
                    for (int h = 115; h <= k; h++)//依照辛工的要求，修改成具体数值，并修改下面代码
                    {
                        string readStr = table.Rows[h][1].ToString();
                        string[] strs = readStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);//将读取的字符串按"制表符/t“和””“分割成数组
                        DataRow dr = table1.NewRow();//创建数据行
                        dr["Depth"] = float.Parse(strs[1]);
                        dr["Time"] = Time;
                        dr["TM"] = float.Parse(strs[2]);
                        table1.Rows.Add(dr);//将创建的数据行添加到table中
                    }
                }
                catch //(Exception se)
                {
                    // table1 = null;
                    //writelog.WriteLog("该文件不可读1" + se+filename);
                }
                return table1;
            }

        }
    }
}
