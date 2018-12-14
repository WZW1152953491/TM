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
using System.Collections;
using ZedGraph;
using System.Threading;


namespace TMCurve.MyClass
{
    class drawAttribute
    {
        public static string DDtitle;
        public static string DDXtitle;
        public static string DDYtitle;

        public static string DTtitle;
        public static string DTXtitle;
        public static string DTYtitle;

        public static string GDtitle;
        public static string GDXtitle;
        public static string GDYtitle;

        public static string GTtitle;
        public static string GTXtitle;
        public static string GTYtitle;

        public static string Welltitle;

        public static string realDTtitle;
        public static string realDTXtitle;
        public static string realDTYtitle;

        public static string realGTtitle;
        public static string realGTXtitle;
        public static string realGTYtitle;

        public static float wellZero=150;

        public static int Linenum = 1;
        public static int strNun = 2500;// 从取文档中数据的行数，默认为两千五

        public static int getStrNum()
        {

            int num ;
            if (MainForm.getInstance().TextstrNum.Text == null)
            {
                num = 2500;
            }
            else
            {
                num = int.Parse(MainForm.getInstance().TextstrNum.Text);
            }
            return num;
        }
        public static void baocunAttribute()
        {
            if (shuxing.getInstance().Wellzero.Text == "")
            {
                wellZero = 150;
            }
            else {
                wellZero = float.Parse(shuxing.getInstance().Wellzero.Text);
            }
            if (shuxing.getInstance().Line.Checked == true)
            {
                Linenum = 1;
            }
            else
            {
                Linenum =2;
            }
            //DTSDep曲线
            if (MainForm.getInstance().tabControl1.SelectedIndex ==0)
            {
                ZedGraph.GraphPane gpDTS1 = MainForm.getInstance().zgcDep.GraphPane;
                if (shuxing.getInstance().sxtitle.Text == "")
                {
                    gpDTS1.Title.Text = "LD27-2平台 A22H井 DTS时间温度曲线"; //图表轴名称
                    DDtitle = "LD27-2平台 A22H井 DTS时间温度曲线"; //图表轴名称
                }
                else
                {
                    gpDTS1.Title.Text = shuxing.getInstance().sxtitle.Text;
                    DDtitle = shuxing.getInstance().sxtitle.Text;
                }
                if (shuxing.getInstance().sxXtitle.Text == "")
                {
                    gpDTS1.XAxis.Title.Text = "时间"; 
                    DDXtitle = "时间"; //图表轴名称
                }
                else
                {
                    gpDTS1.XAxis.Title.Text = shuxing.getInstance().sxXtitle.Text;
                    DDXtitle = shuxing.getInstance().sxXtitle.Text;
                }
                if ( shuxing.getInstance().sxYtitle.Text == "")
                {
                    gpDTS1.YAxis.Title.Text = "温度(℃)"; 
                    DDYtitle = "温度(℃)"; //图表轴名称
                }
                else
                {
                  
                    gpDTS1.YAxis.Title.Text = shuxing.getInstance().sxYtitle.Text;
                    DDYtitle = shuxing.getInstance().sxYtitle.Text;
                }
                MainForm.getInstance().zgcDep.Refresh();
            }
           
            //DTSTime曲线
            if (MainForm.getInstance().tabControl1.SelectedIndex == 1)
            {
                ZedGraph.GraphPane gpDTS2 = MainForm.getInstance().zgcTime.GraphPane;
                if (shuxing.getInstance().sxtitle.Text == "")
                {
                    gpDTS2.Title.Text = "LD27-2平台 A22H井 DTS深度温度曲线"; //图表轴名称
                    DTtitle = "LD27-2平台 A22H井 DTS深度温度曲线"; //图表轴名称
                }
                else
                {
                    gpDTS2.Title.Text = shuxing.getInstance().sxtitle.Text;
                    DTtitle = shuxing.getInstance().sxtitle.Text;
                }
                if (shuxing.getInstance().sxXtitle.Text == "")
                {
                    gpDTS2.XAxis.Title.Text = "深度(m)";
                    DTXtitle = "深度(m)"; //图表轴名称
                }
                else
                {
                    gpDTS2.XAxis.Title.Text = shuxing.getInstance().sxXtitle.Text;
                    DTXtitle = shuxing.getInstance().sxXtitle.Text;
                }
                if (shuxing.getInstance().sxYtitle.Text == "")
                {
                    gpDTS2.YAxis.Title.Text = "温度(℃)";
                    DTYtitle = "温度(℃)"; //图表轴名称
                }
                else
                {

                    gpDTS2.YAxis.Title.Text = shuxing.getInstance().sxYtitle.Text;
                    DTYtitle = shuxing.getInstance().sxYtitle.Text;
                }
                MainForm.getInstance().zgcTime.Refresh();
            }
            if (MainForm.getInstance().tabControl1.SelectedIndex == 2)
            {
                ZedGraph.GraphPane gpGrat1 = MainForm.getInstance().GDep.GraphPane;
                if (shuxing.getInstance().sxtitle.Text == "")
                {
                    gpGrat1.Title.Text = "LD27-2平台 A22H井 光栅时间温度曲线"; //图表轴名称
                    GDtitle = "LD27-2平台 A22H井 光栅时间温度曲线"; //图表轴名称
                }
                else
                {
                    gpGrat1.Title.Text = shuxing.getInstance().sxtitle.Text;
                    GDtitle = shuxing.getInstance().sxtitle.Text;
                }
                if (shuxing.getInstance().sxXtitle.Text == "")
                {
                    gpGrat1.XAxis.Title.Text = "时间";
                    GDXtitle = "时间"; //图表轴名称
                }
                else
                {
                    gpGrat1.XAxis.Title.Text = shuxing.getInstance().sxXtitle.Text;
                    GDXtitle = shuxing.getInstance().sxXtitle.Text;
                }
                if (shuxing.getInstance().sxYtitle.Text == "")
                {
                    gpGrat1.YAxis.Title.Text = "温度(℃)";
                    GDYtitle = "温度(℃)"; //图表轴名称
                }
                else
                {

                    gpGrat1.YAxis.Title.Text = shuxing.getInstance().sxYtitle.Text;
                    GDYtitle = shuxing.getInstance().sxYtitle.Text;
                }
                MainForm.getInstance().GDep.Refresh();
            }

            //DTSTime曲线
            if (MainForm.getInstance().tabControl1.SelectedIndex == 3)
            {
                ZedGraph.GraphPane gpGrat2 = MainForm.getInstance().GTime.GraphPane;
                if (shuxing.getInstance().sxtitle.Text == "")
                {
                    gpGrat2.Title.Text = "LD27-2平台 A22H井 光栅深度温度曲线"; //图表轴名称
                    GTtitle = "LD27-2平台 A22H井 光栅深度温度曲线"; //图表轴名称
                }
                else
                {
                    gpGrat2.Title.Text = shuxing.getInstance().sxtitle.Text;
                    GTtitle = shuxing.getInstance().sxtitle.Text;
                }
                if (shuxing.getInstance().sxXtitle.Text == "")
                {
                    gpGrat2.XAxis.Title.Text = "深度(m)";
                    GTXtitle = "深度(m)"; //图表轴名称
                }
                else
                {
                    gpGrat2.XAxis.Title.Text = shuxing.getInstance().sxXtitle.Text;
                    GTXtitle = shuxing.getInstance().sxXtitle.Text;
                }
                if (shuxing.getInstance().sxYtitle.Text == "")
                {
                    gpGrat2.YAxis.Title.Text = "温度(℃)";
                    GTYtitle = "温度(℃)"; //图表轴名称
                }
                else
                {

                    gpGrat2.YAxis.Title.Text = shuxing.getInstance().sxYtitle.Text;
                    GTYtitle = shuxing.getInstance().sxYtitle.Text;
                }
                MainForm.getInstance().GTime.Refresh();
            }
            //Well曲线
           if (MainForm.getInstance().tabControl1.SelectedIndex == 4)
            {
                ZedGraph.GraphPane myPane = MainForm.getInstance().WellTM.GraphPane;
                if (shuxing.getInstance().sxtitle.Text == null)
                {
                    myPane.Title.Text = "LD27-2平台 A22H井 管柱图";
                    Welltitle = "LD27-2平台 A22H井 管柱图"; //图表轴名称
                }
                else
                {
                    myPane.Title.Text = shuxing.getInstance().sxtitle.Text;
                    Welltitle = shuxing.getInstance().sxtitle.Text;
                }
                MainForm.getInstance().WellTM.Refresh();
            }

           if (MainForm.getInstance().tabControl1.SelectedIndex == 5)
           {
               ZedGraph.GraphPane gpDTS3 = MainForm.getInstance().DTSReal.GraphPane;
               if (shuxing.getInstance().sxtitle.Text == "")
               {
                   gpDTS3.Title.Text = "LD27-2平台 A22H井 DTS深度温度曲线"; //图表轴名称
                   realDTtitle = "LD27-2平台 A22H井 DTS深度温度曲线"; //图表轴名称
               }
               else
               {
                   gpDTS3.Title.Text = shuxing.getInstance().sxtitle.Text;
                   realDTtitle = shuxing.getInstance().sxtitle.Text;
               }
               if (shuxing.getInstance().sxXtitle.Text == "")
               {
                   gpDTS3.XAxis.Title.Text = "深度(m)";
                   realDTXtitle = "深度(m)"; //图表轴名称
               }
               else
               {
                   gpDTS3.XAxis.Title.Text = shuxing.getInstance().sxXtitle.Text;
                   realDTXtitle = shuxing.getInstance().sxXtitle.Text;
               }
               if (shuxing.getInstance().sxYtitle.Text == "")
               {
                   gpDTS3.YAxis.Title.Text = "温度(℃)";
                   realDTYtitle = "温度(℃)"; //图表轴名称
               }
               else
               {

                   gpDTS3.YAxis.Title.Text = shuxing.getInstance().sxYtitle.Text;
                   realDTYtitle = shuxing.getInstance().sxYtitle.Text;
               }
               MainForm.getInstance().DTSReal.Refresh();
           }
           if (MainForm.getInstance().tabControl1.SelectedIndex == 6)
           {
               ZedGraph.GraphPane gpGrat3 = MainForm.getInstance().GratReal.GraphPane;
               if (shuxing.getInstance().sxtitle.Text == "")
               {
                   gpGrat3.Title.Text = "LD27-2平台 A22H井 光栅深度温度曲线"; //图表轴名称
                   realGTtitle = "LD27-2平台 A22H井 光栅深度温度曲线"; //图表轴名称
               }
               else
               {
                   gpGrat3.Title.Text = shuxing.getInstance().sxtitle.Text;
                   realGTtitle = shuxing.getInstance().sxtitle.Text;
               }
               if (shuxing.getInstance().sxXtitle.Text == "")
               {
                   gpGrat3.XAxis.Title.Text = "深度(m)";
                   realGTXtitle = "深度(m)"; //图表轴名称
               }
               else
               {
                   gpGrat3.XAxis.Title.Text = shuxing.getInstance().sxXtitle.Text;
                   realGTXtitle = shuxing.getInstance().sxXtitle.Text;
               }
               if (shuxing.getInstance().sxYtitle.Text == "")
               {
                   gpGrat3.YAxis.Title.Text = "温度(℃)";
                   realGTYtitle = "温度(℃)"; //图表轴名称
               }
               else
               {

                   gpGrat3.YAxis.Title.Text = shuxing.getInstance().sxYtitle.Text;
                   realGTYtitle = shuxing.getInstance().sxYtitle.Text;
               }
               MainForm.getInstance().GratReal.Refresh();
           }
           
       
        }
        public static void morenAttribute()
        {
            //DTS
            shuxing.getInstance().Wellzero.Text ="150";
            shuxing.getInstance().Line.Checked = true;
             Linenum = 1;
            if (MainForm.getInstance().tabControl1.SelectedIndex == 0)
            {
                shuxing.getInstance().sxXtitle.Text = "时间";
                shuxing.getInstance().sxYtitle.Text = "温度(℃)";
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 DTS时间温度曲线";
            }

            if (MainForm.getInstance().tabControl1.SelectedIndex == 1)
            {
                shuxing.getInstance().sxXtitle.Text = "深度(m)";
                shuxing.getInstance().sxYtitle.Text = "温度(℃)";
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 DTS深度温度曲线";
            }
            if (MainForm.getInstance().tabControl1.SelectedIndex == 2)
            {
                shuxing.getInstance().sxXtitle.Text = "时间";
                shuxing.getInstance().sxYtitle.Text = "温度(℃)";
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 光栅时间温度曲线";
            }

            if (MainForm.getInstance().tabControl1.SelectedIndex == 3)
            {
                shuxing.getInstance().sxXtitle.Text = "深度(m)";
                shuxing.getInstance().sxYtitle.Text = "温度(℃)";
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 光栅深度温度曲线";
            }
            if (MainForm.getInstance().tabControl1.SelectedIndex == 4)
            {
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 管柱图"; //图表轴名称
            } 
            if (MainForm.getInstance().tabControl1.SelectedIndex == 5)
            {
                shuxing.getInstance().sxXtitle.Text = "深度(m)";
                shuxing.getInstance().sxYtitle.Text = "温度(℃)";
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 DTS深度温度曲线";
            } 
            if (MainForm.getInstance().tabControl1.SelectedIndex == 6)
            {
                shuxing.getInstance().sxXtitle.Text = "深度(m)";
                shuxing.getInstance().sxYtitle.Text = "温度(℃)";
                shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 光栅深度温度曲线";
            } 
     
        }
        public static string getAttribute(int curLine,string filename)
        {
           // FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Read);//创建写入文件 
            string readStr=null;
            string sb = null;
            curLine = curLine + 2;
            using (StreamReader sr = new StreamReader(filename, Encoding.Default))//读取每一行数据到临时表
            {
                readStr = sr.ReadLine();
             
                for (int i = 1; readStr != null; i++)
                {
                   
                    if (i != curLine - 1)
                    {
                        readStr = sr.ReadLine();
                    }
                       // readStr = sr.ReadLine();
                    else
                    {
                       // sr.ReadLine();
                        readStr = sr.ReadLine();
                        sb = readStr;
                        //line = newLineValue;
                    }
                }
                
                //while (!sr.EndOfStream)
                //{
                //    readStr = sr.ReadLine();//读取一行数据
                //}
            }

            return sb;
        }
    }
}
