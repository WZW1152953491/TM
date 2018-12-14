using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections;
using ZedGraph;
using System.Threading;
using System.Threading.Tasks;

namespace TMCurve.MyClass
{
    class drawingGDep : MyClass
    {
        //public static MySqlConnection mycon;
       // public static String GratDepth;//单项深度
        public static DateTime GratDepTime1;
        public static DateTime GratDepTime2;//区间时间2
        public static string str;
       public static  bool a = false;
      
        public static int bu = 0;


        private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        {
            using (Graphics gc = MainForm.getInstance().GDep.CreateGraphics())
            using (Pen pen = new Pen(Color.Gray))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                RectangleF rect = MainForm.getInstance().GDep.GraphPane.Chart.Rect;
               
                //确保在画图区域
                if (rect.Contains(e.Location))
                {
                    MainForm.getInstance().GDep.Refresh();
                    gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
                    gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);
                }
            }
        }
        public static void DrawingGratDep()//画时间温度曲线
        {

            MyThread myt = new MyThread();
          
            string messageError = null;
            string GratDepth = MainForm.getInstance().textBox3.Text;
            String SQLstr = getstr();//获取str
            MainForm.getInstance().groupBox1.Enabled = false;
            MainForm.getInstance().GDep.Enabled = false;
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            string button = MainForm.getInstance().number.Text;
            float wellzero = ZedGraphClass.getWellZero();
            if (SQLstr != null)
            {

                DataTable SQLName = getTNameTable(GratDepTime1, GratDepTime2);//获取需要使用的表名称
                if (SQLName.Rows.Count != 0)
                {
                    ArrayList SQLList = MyDataTable.getDepth(wellzero, GratDepth);//获取深度值
                    if (SQLList.Count != 0)
                    {
                        SQLList = ZedGraphClass.getNewDepth(SQLList);//去重
                        ZedGraph.GraphPane gp = MainForm.getInstance().GDep.GraphPane;
                        gp.GraphObjList.Clear();
                        gp.CurveList.Clear();
                        if (MainForm.getInstance().label1.Text == "1")
                        {
                            MainForm.getInstance().GDep.IsEnableVZoom = true;//Y轴缩放
                            MainForm.getInstance().GDep.IsEnableHZoom = true;//x轴缩放

                        }
                        else
                        {
                            MainForm.getInstance().GDep.IsEnableVZoom = false;//禁止Y轴缩放
                            MainForm.getInstance().GDep.IsEnableHZoom = true;//x轴缩放
                        }
                        MainForm.getInstance().GDep.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerDep);//设置节点信息显示样式
                        //MainForm.getInstance().GDep.IsShowHScrollBar = true;  //是否显示横向滚动条。
                        //MainForm.getInstance().GDep.ZoomStepFraction = 0;//不允许鼠标放大缩小
                        MainForm.getInstance().GDep.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
                        MainForm.getInstance().GDep.IsShowPointValues = true; //显示节点坐标值
                        MainForm.getInstance().GDep.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。
                        if (MainForm.getInstance().GratDepTM1.Text != "" && MainForm.getInstance().GratDepTM2.Text != "")
                        {
                            gp.YAxis.Scale.Min = float.Parse(MainForm.getInstance().GratDepTM1.Text);
                            gp.YAxis.Scale.Max = float.Parse(MainForm.getInstance().GratDepTM2.Text);
                        }
                        else
                        {
                            gp.YAxis.Scale.MaxAuto = true;//自动设置大小
                            gp.YAxis.Scale.MinAuto = true;
                        }
                        MainForm.getInstance().GDep.IsAutoScrollRange = false;
                        gp.XAxis.Scale.Format = "yyyy-MM-dd HH:mm:ss";//横轴格式
                        gp.XAxis.Type = AxisType.Date;//格式
                        string[] hn = new string[SQLList.Count];//折现的标签
                        if (button == "two")
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                gp.GraphObjList.Clear();
                                gp.CurveList.Clear();//清除上一步画的图

                                for (int i = 0; i < SQLList.Count; i++)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    DataTable dtValue = new DataTable();
                                    string SQLque = "RecordTime";
                                    dtValue = MyDataTable.getDataTable(SQLName, Convert.ToInt32(SQLList[i]), SQLstr, SQLque, mycon);
                                    float e = Convert.ToInt32(SQLList[i].ToString()) - wellzero;
                                    hn[i] = e + "m";
                                    //hn[i] = SQLList[i] + "m";
                                    PointPairList list1 = new PointPairList();
                                    for (int j = 0; j < dtValue.Rows.Count; j++)
                                    {
                                        int bili = 1;
                                        if (dtValue.Rows.Count > 1000)
                                        {
                                            bili = dtValue.Rows.Count / 500;
                                        }
                                        if (j % bili == 0)
                                        {

                                            // string a = dt.Rows[j]["RecordTime"].ToString();
                                            double x = (double)new XDate((DateTime)dtValue.Rows[j]["RecordTime"]);
                                            //double x = (double)new XDate((DateTime)dt.Rows[j]["RecordTime"]);
                                            float y = float.Parse(dtValue.Rows[j]["TM"].ToString());
                                            list1.Add(x, y);
                                        }
                                        //TextObj text = new TextObj("shiji", x, y);
                                        //gp.GraphObjList.Add(text);
                                    }

                                    if (list1.Count == 0 && k == 0)//如果曲线没有数据或缺少数据
                                    {
                                        //MessageBox.Show("曲线不存在");
                                        messageError += "深度" + SQLList[i] + "m无数据\n";
                                        continue;
                                    }
                                    else
                                    {
                                        Color co = ZedGraphClass.GetColor(i);
                                        LineItem _lineitem2 = gp.AddCurve(hn[i], list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
                                        //_lineitem2.Label.IsVisible = false;//名称不见的一种形式
                                        _lineitem2.Line.Width = 2.0F;//线的宽度
                                        //节点设置
                                        if (drawAttribute.Linenum == 2)
                                        {
                                            _lineitem2.Line.IsVisible = false;
                                        }
                                        _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                        _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                                        gp.AxisChange();
                                        MainForm.getInstance().GDep.Refresh();
                                    }
                                }
                            }

                        }
                        else
                        {

                            for (int i = 0; i < SQLList.Count; i++)//先做深度循环
                            {

                             
                                DataTable dtValue = new DataTable();
                                string SQLque = "RecordTime";
                                dtValue = MyDataTable.getDataTable(SQLName, Convert.ToInt32(SQLList[i]), SQLstr, SQLque, mycon);
                                float e = Convert.ToInt32(SQLList[i].ToString()) - wellzero;
                                hn[i] = e + "m";
                                PointPairList list1 = new PointPairList();
                                for (int j = 0; j < dtValue.Rows.Count; j++)
                                {

                                    int bili = 1;
                                    if (dtValue.Rows.Count > 1000)
                                    {
                                        bili = dtValue.Rows.Count / 500;
                                    }
                                    if (j % bili == 0)
                                    {
                                        double x = (double)new XDate((DateTime)dtValue.Rows[j]["RecordTime"]);
                                        //  string a = dt.Rows[j]["RecordTime"].ToString();
                                        double y = double.Parse(dtValue.Rows[j]["TM"].ToString());

                                        list1.Add(x, y);
                                    }
                                }
                                if (list1.Count == 0)//如果曲线没有数据
                                {
                                    messageError += "深度" + SQLList[i] + "m无数据\n";
                                    continue;
                                }
                                else
                                {
                                    Color co = ZedGraphClass.GetColor(i);
                                    LineItem _lineitem2 = gp.AddCurve(hn[i], list1, co, SymbolType.Circle);
                                    _lineitem2.Line.Width = 2.0F;//线的宽度
                                    string la = _lineitem2.Label.Text.ToString();
                                    //节点设置
                                    if (drawAttribute.Linenum == 2)
                                    {
                                        _lineitem2.Line.IsVisible = false;
                                    }
                                    _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                    _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                                    gp.AxisChange();
                                    MainForm.getInstance().GDep.Refresh();
                                }

                            }


                        }
                    }
                    else
                    {
                        //MessageBox.Show("深度输入不正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("请填写深度！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    //MessageBox.Show("所选时间区间内没有数据，请更改时间区域！"); //没有表
                    MessageBox.Show("所选时间区间内没有数据，请更改时间区域！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (messageError != null)
                {
                    MessageBox.Show("以下深度点无数据！\n" + messageError, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("时间区间选择不正确，请修改！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MainForm.getInstance().groupBox1.Enabled = true;
            MainForm.getInstance().GDep.Enabled = true;
            mycon.Close();
            mycon.Dispose();
            SQLstr = null;
        }

        public static string getstr()//获取SQL语句
        {

            if (MainForm.getInstance().GratDepTime2.Value >= MainForm.getInstance().GratDepTime1.Value)//如果时间正确
            {
                GratDepTime1 = MainForm.getInstance().GratDepTime1.Value;
                GratDepTime2 = MainForm.getInstance().GratDepTime2.Value;
                str = " WHERE RecordTime BETWEEN  \'" + GratDepTime1 + "\' and \'" + GratDepTime2 + "\'";
            }

            else
            {
                str = null;
            }
            return str;
        }

  
     
        private static string getTimeString(ArrayList TimeC)
        {
            string TimeString;
            TimeString = "\'" + TimeC[0].ToString() + "\'";
            for (int i = 1; i < TimeC.Count; i++)
            {
                TimeString = TimeString + ",\'" + TimeC[i].ToString() + "\'";

            }
            return TimeString;
        }
        public static DataTable getTNameTable(DateTime a, DateTime b)//依照两个时间点，获取表名称
        {

            MySqlConnection mycon = MyClass.getMycon();
            DataTable dt = new DataTable();
            string str = "select distinct folderTable from allgrat_data where folderTime >= '" + a + "' and folderTIme<='" + b + "' order by folderTime";
            dt = MyClass.getDataTable(str, mycon);
            mycon.Clone();
            mycon.Dispose();
            return dt;
        }
        public static bool getWait()
        {
            Thread.Sleep(2000);
            a = true;
            return a;
        }
    }
}


