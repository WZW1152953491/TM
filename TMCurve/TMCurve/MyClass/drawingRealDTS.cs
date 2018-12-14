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
    class drawingRealDTS
    {
        //private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        //{
        //    using (Graphics gc = MainForm.getInstance().DTSReal.CreateGraphics())
        //    using (Pen pen = new Pen(Color.Gray))
        //    {
        //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //        RectangleF rect = MainForm.getInstance().DTSReal.GraphPane.Chart.Rect;
        //        //确保在画图区域
        //        if (rect.Contains(e.Location))
        //        {
        //            MainForm.getInstance().DTSReal.Refresh();
        //            gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
        //            gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);
        //        }
        //    }
        //}
       
        public static void drawRealDTS(List<DataTable>  dt)
        {
            float wellzero = ZedGraphClass.getWellZero();
            ZedGraph.GraphPane gp = MainForm.getInstance().DTSReal.GraphPane;
            MainForm.getInstance().DTSReal.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerTime);//设置节点信息显示样式
            //MainForm.getInstance().DTSReal.IsShowHScrollBar = true;//横向滚动条
           // MainForm.getInstance().DTSReal.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
            MainForm.getInstance().DTSReal.IsShowPointValues = true;//
            MainForm.getInstance().DTSReal.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。
            gp.GraphObjList.Clear();
            gp.CurveList.Clear();
            //int Linenumber = Convert.ToInt32(MainForm.getInstance().DTSLineNumber.Text);
            //int k = 0;
            //if (dt.Count - Linenumber < 0)
            //{

            //}
            //else
            //{
            //    k = dt.Count - Linenumber;
            //}
            for (int i = 1; i < dt.Count; i++)//从第一条线开始，并且两条线
            {
                DataTable table = dt[i];
              
                string Linename = table.Rows[0][0].ToString();
                PointPairList list1 = new PointPairList();
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    double x;
                    float y;
                    //X轴减去部分井口数据。
                    x = float.Parse(table.Rows[j][1].ToString()) - wellzero;
                    y = float.Parse(table.Rows[j][2].ToString());
                    list1.Add(x, y);
                }
                if (list1.Count == 0)//如果曲线没有数据
                {
                    continue;
                }
                else
                {
                    Color co = ZedGraphClass.GetColor(i);
                    LineItem _lineitem2 = gp.AddCurve(Linename, list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
                    _lineitem2.Line.Width = 2.0F;//线的宽度
                    string la = _lineitem2.Label.Text.ToString();
                    _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                    _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                    //gp.AxisChange();//若是
                }
            }
            gp.AxisChange();//若是放到上面的那一行，因为数据太多，会有延迟，导致图形颜色不断变化（一条线一条线的画）
            MainForm.getInstance().DTSReal.Refresh();
        }

        public static void drawRealDTS1(DataTable table)
        {
            float wellzero = ZedGraphClass.getWellZero();
         
            ZedGraph.GraphPane gp = MainForm.getInstance().DTSReal.GraphPane;
            MainForm.getInstance().DTSReal.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerTime);//设置节点信息显示样式
            //MainForm.getInstance().DTSReal.IsShowHScrollBar = true;//横向滚动条
           // MainForm.getInstance().DTSReal.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
            MainForm.getInstance().DTSReal.IsShowPointValues = true;//
            MainForm.getInstance().DTSReal.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。
            gp.GraphObjList.Clear();
            gp.CurveList.Clear();
            //int Linenumber = Convert.ToInt32(MainForm.getInstance().DTSLineNumber.Text);
            //int k = 0;
            //if (dt.Count - Linenumber < 0)
            //{

            //}
            //else
            //{
            //    k = dt.Count - Linenumber;
            //}
           
               
                if (table.Rows.Count == 0)//如果曲线没有数据
                {
                    
                }
                else
                {
                    PointPairList list1 = new PointPairList();
                    string Linename = table.Rows[0][0].ToString();
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        double x;
                        float y;
                        //X轴减去部分井口数据。
                        x = float.Parse(table.Rows[j][1].ToString()) - wellzero;
                        y = float.Parse(table.Rows[j][2].ToString());
                        list1.Add(x, y);
                    }
                    Color co = ZedGraphClass.GetColor(0);
                    LineItem _lineitem2 = gp.AddCurve(Linename, list1, ZedGraphClass.GetColor(0), SymbolType.Circle);
                    _lineitem2.Line.Width = 2.0F;//线的宽度
                    string la = _lineitem2.Label.Text.ToString();
                    _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                    _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                    //gp.AxisChange();//若是
                }
            
            gp.AxisChange();//若是放到上面的那一行，因为数据太多，会有延迟，导致图形颜色不断变化（一条线一条线的画）
            MainForm.getInstance().DTSReal.Refresh();
        }
    }
}
