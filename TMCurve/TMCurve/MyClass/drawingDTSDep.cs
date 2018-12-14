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
    class drawingDTSDep:MyClass
    {
       
        public static DateTime DintervalTime1;
        public static DateTime DintervalTime2;//区间时间2
        public static string str;
       
      
        private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        {
            using (Graphics gc = MainForm.getInstance().zgcDep.CreateGraphics())
            using (Pen pen = new Pen(Color.Gray))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                RectangleF rect = MainForm.getInstance().zgcDep.GraphPane.Chart.Rect;
                //确保在画图区域
                if (rect.Contains(e.Location))
                {
                    MainForm.getInstance().zgcDep.Refresh();
                    gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
                    gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);
                }
            }
        }
        public static void DrawingDep()//画时间温度曲线
        {
            //获取时间的查询条件
            string messageError=null;
            String SQLstr = getSQLstr();
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            //获取井口位置
            float wellzero = ZedGraphClass.getWellZero();
            //获取循环（单循环还是多循环）
            string button = MainForm.getInstance().number.Text;
            //画图图禁止菜单和画图区域
            MainForm.getInstance().groupBox2.Enabled = false;
            MainForm.getInstance().zgcDep.Enabled = false;
            //新建数据库连接
           string  DsingleDepth = MainForm.getInstance().DsingleDepth.Text;
            if (SQLstr != null)
            {
               
                DataTable SQLName = getTNameTable(DintervalTime1, DintervalTime2);//获取需要使用的表名称
                if (SQLName.Rows.Count != 0)
                {
                    ArrayList SQLList = MyDataTable.getDepth(wellzero, DsingleDepth);//获取深度值
                        if (SQLList.Count <= 15 && SQLList.Count > 0)//15条线之内
                        {
                            SQLList = ZedGraphClass.getNewDepth(SQLList);//去重
                            ZedGraph.GraphPane gp = MainForm.getInstance().zgcDep.GraphPane;
                            //gp.CurveList.Clear();//清除上一步画的图
                            gp.GraphObjList.Clear();
                            gp.CurveList.Clear();
                            //是否X、Y轴缩放的定义
                            if (MainForm.getInstance().label1.Text == "1")
                            {
                                MainForm.getInstance().zgcDep.IsEnableVZoom = true;//Y轴缩放
                                MainForm.getInstance().zgcDep.IsEnableHZoom = true;//x轴缩放
                            }
                            else
                            {

                                MainForm.getInstance().zgcDep.IsEnableVZoom = false;//禁止Y轴缩放
                                MainForm.getInstance().zgcDep.IsEnableHZoom = true;//x轴缩放
                            }
                            MainForm.getInstance().zgcDep.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerDep);//设置节点信息显示样式
                            MainForm.getInstance().zgcDep.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
                            MainForm.getInstance().zgcDep.IsShowPointValues = true; //显示节点坐标值
                            MainForm.getInstance().zgcDep.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。
                            if (MainForm.getInstance().DintervalTM1.Text != "" && MainForm.getInstance().DintervalTM2.Text != "")
                            {
                                gp.YAxis.Scale.Min = float.Parse(MainForm.getInstance().DintervalTM1.Text);
                                gp.YAxis.Scale.Max = float.Parse(MainForm.getInstance().DintervalTM2.Text);
                            }
                            else
                            {
                                gp.YAxis.Scale.MaxAuto = true;//自动设置大小
                                gp.YAxis.Scale.MinAuto = true;
                            }
                            MainForm.getInstance().zgcDep.IsAutoScrollRange = false;
                            //坐标轴刻度格式
                            gp.XAxis.Scale.Format = "yyyy-MM-dd HH:mm:ss";//横轴格式
                            gp.XAxis.Type = AxisType.Date;//格式
                            string[] hn = new string[SQLList.Count];//折线的标签
                            if (button == "two")//循环作图
                            {
                                for (int xunhuan = 0; xunhuan < 4; xunhuan++)
                                {
                                    //清除上一步画的图
                                    gp.GraphObjList.Clear();
                                    gp.CurveList.Clear();

                                    for (int i = 0; i < SQLList.Count; i++)
                                    {
                                        System.Threading.Thread.Sleep(1000);
                                        PointPairList list1 = new PointPairList();
                                        DataTable dtValue = new DataTable();
                                        string SQLque = "RecordTime";
                                        dtValue = MyDataTable.getDataTable(SQLName, Convert.ToInt32(SQLList[i]), SQLstr, SQLque, mycon);
                                        float e = Convert.ToInt32(SQLList[i].ToString()) - wellzero;
                                        hn[i] = e + "m";
                                        for (int k = 0; k < dtValue.Rows.Count; k++)
                                        {
                                            int bili = 1;
                                            if (dtValue.Rows.Count > 1000)
                                            {
                                                bili = dtValue.Rows.Count / 500;
                                            }
                                            if (k % bili == 0)
                                            {
                                                double x = (double)new XDate((DateTime)dtValue.Rows[k]["RecordTime"]);
                                                string a = dtValue.Rows[k]["RecordTime"].ToString();
                                                double y = double.Parse(dtValue.Rows[k]["TM"].ToString());
                                                list1.Add(x, y);
                                            }
                                        }
                                        if (list1.Count == 0 && xunhuan == 0)//如果曲线没有数据
                                        {
                                            messageError += "深度" + SQLList[i] + "m无数据\n";
                                            continue;
                                        }
                                        else
                                        {
                                            Color co = ZedGraphClass.GetColor(i);
                                            LineItem _lineitem2 = gp.AddCurve(hn[i], list1, co, SymbolType.Circle);
                                            _lineitem2.Line.Width = 2.0F;//线的宽度
                                            //节点设置
                                            if (drawAttribute.Linenum == 2)
                                            {
                                                _lineitem2.Line.IsVisible = false;
                                            }
                                            _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                            _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                                            gp.AxisChange();
                                            MainForm.getInstance().zgcDep.Refresh();

                                        }

                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SQLList.Count; i++)
                                {
                                    PointPairList list1 = new PointPairList();
                                    DataTable dtValue = new DataTable();
                                    string SQLque = "RecordTime";
                                    dtValue = MyDataTable.getDataTable(SQLName, Convert.ToInt32(SQLList[i]), SQLstr, SQLque, mycon);
                                    float e = Convert.ToInt32(SQLList[i].ToString()) - wellzero;
                                    hn[i] = e + "m";
                                    for (int k = 0; k < dtValue.Rows.Count; k++)
                                    {
                                        int bili = 1;
                                        if (dtValue.Rows.Count > 1000)
                                        {
                                            bili = dtValue.Rows.Count / 500;
                                        }
                                        if (k % bili == 0)
                                        {
                                            double x = (double)new XDate((DateTime)dtValue.Rows[k]["RecordTime"]);
                                            string a = dtValue.Rows[k]["RecordTime"].ToString();
                                            double y = double.Parse(dtValue.Rows[k]["TM"].ToString());
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
                                        //节点设置
                                        if (drawAttribute.Linenum == 2)
                                        {
                                            _lineitem2.Line.IsVisible = false;
                                        }
                                        _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                        _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                                        gp.AxisChange();
                                        MainForm.getInstance().zgcDep.Refresh();
                                    }
                                }
                            }
                        }
                        else if (SQLList.Count >15)
                        {
                            //MessageBox.Show("深度区间太大，曲线条数大于15");
                            MessageBox.Show("深度区间太大，曲线条数大于15！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                           // MessageBox.Show("在所选时间区间内数据库中无数据");//表中无数据  
                            MessageBox.Show("请填写深度！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }            
                }
                else
                {
                  //  MessageBox.Show("所选时间区间内没有数据，请更改时间区域！"); //没有表
                    MessageBox.Show("所选时间区间内没有数据，请更改时间区域！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);     
                }
                if (messageError != null)
                {
                    MessageBox.Show("以下深度点无数据！\n" + messageError, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
               // MessageBox.Show("时间区间选择不正确，请修改！");
                MessageBox.Show("时间区间选择不正确，请修改！" , "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MainForm.getInstance().groupBox2.Enabled = true;
            MainForm.getInstance().zgcDep.Enabled = true;
            mycon.Close();
            mycon.Dispose();
            SQLstr = null;
        }
        public static string getSQLstr()//获取SQL限制语句1
        {
            DintervalTime1 = MainForm.getInstance().DintervalTime1.Value;
            DintervalTime2 = MainForm.getInstance().DintervalTime2.Value;
            if (MainForm.getInstance().DintervalTime2.Value >= MainForm.getInstance().DintervalTime1.Value)//如果时间正确
            {
                str = " WHERE RecordTime BETWEEN  \'" + DintervalTime1 + "\' and \'" + DintervalTime2 + "\'";
            }
            else
            {
                str = null;
            }
            return str;
        }

        //依照两个时间点，获取表名称
        public static DataTable getTNameTable(DateTime a,DateTime b)
        {
            MySqlConnection mycon = MyClass.getMycon();
            DataTable dt=new DataTable();
            string str = "select distinct folderTable from alltemporpary_data where folderTime >= '" + a + "' and folderTIme<='" + b + "' order by folderTime";
            dt = MyClass.getDataTable(str, mycon);
            mycon.Clone();
            mycon.Dispose();
            return dt;
        }
    }
}

