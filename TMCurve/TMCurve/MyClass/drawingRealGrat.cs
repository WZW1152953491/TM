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
    class drawingRealGrat
    {
        //private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        //{
        //    using (Graphics gc = MainForm.getInstance().GratReal.CreateGraphics())
        //    using (Pen pen = new Pen(Color.Gray))
        //    {
        //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //        RectangleF rect = MainForm.getInstance().GratReal.GraphPane.Chart.Rect;
        //        //确保在画图区域
        //        if (rect.Contains(e.Location))
        //        {
        //            MainForm.getInstance().GratReal.Refresh();
        //            gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
        //            gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);
        //        }
        //    }
        //}
        public static void drawRealGrat(List<DataTable> dt)
        {
            int Linenumber = Convert.ToInt32(MainForm.getInstance().GratLineNumber.Text);
            float wellzero = ZedGraphClass.getWellZero();
            //if (dt.Count <= Linenumber)
            //{
            //    //画图

            //}
            //else
            //{
            //    //达到11的话，删除最初的列表。并作图。
            //    int j = dt.Count - Linenumber;
            //    if (j > 0)
            //    {
            //        for (int i = 0; i < j; i++)
            //        {
            //            dt.RemoveAt(0);
            //        }
            //    }
            //}
            ZedGraph.GraphPane gp = MainForm.getInstance().GratReal.GraphPane;
            MainForm.getInstance().GratReal.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerTime);//设置节点信息显示样式
            //MainForm.getInstance().GratReal.IsShowHScrollBar = true;//横向滚动条
            //MainForm.getInstance().GratReal.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
            MainForm.getInstance().GratReal.IsShowPointValues = true;//
            MainForm.getInstance().GratReal.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。
            gp.GraphObjList.Clear();
            gp.CurveList.Clear();
            int k = 0;
            if (dt.Count - Linenumber < 0)
            {

            }
            else
            {
                k = dt.Count - Linenumber;
            }
            for (int i = k; i < dt.Count; i++)//从第几条线开始，到结束
            {
                DataTable table = dt[i];
                string Linename = table.Rows[0][0].ToString();
                PointPairList list1 = new PointPairList();
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    double x;
                    float y;
                    x = float.Parse(table.Rows[j]["fb"].ToString()) - wellzero;
                    y = float.Parse(table.Rows[j]["fc"].ToString());
                    list1.Add(x, y);
                }
                if (list1.Count == 0)//如果曲线没有数据
                {
                    //MessageBox.Show("曲线不存在");
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
                    gp.AxisChange();
                }
            }
            MainForm.getInstance().GratReal.Refresh();
        }
    }
}
