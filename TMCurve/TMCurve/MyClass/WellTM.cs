using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using ZedGraph;
using System.Threading;
/*利用getstr1方法的到4种情况下的SQL语句，使用work()方法来利用SQL语句得到临时表，然后导出到Execl 2018.2.26完成*/

namespace TMCurve.MyClass
{
    class WellTM: MyClass
    {
        //private static MySqlConnection mycon;
      //  private static int A = 10;
        private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        {
            using (Graphics gc = MainForm.getInstance().WellTM.CreateGraphics())
            using (Pen pen = new Pen(Color.Gray))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                RectangleF rect = MainForm.getInstance().WellTM.GraphPane.Chart.Rect;
                //确保在画图区域
                if (rect.Contains(e.Location))
                {
                    MainForm.getInstance().WellTM.Refresh();
                    gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
                    gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);
                }
            }
        }
        public static string  getWellStr()
        {
            float wellzero = ZedGraphClass.getWellZero();
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            string Str =null;
            //string aa = MainForm.getInstance().WellTime.Value.ToString();
            DateTime aa = MainForm.getInstance().WellTime.Value;//获取时间
            //string name = CreateTable.getTableName(aa, "DTS");//获取文件将要存入的表名称
            //CreateTable.CDataTable(name);//创建表
            string str = "SELECT DISTINCT folderTime from alltemporpary_data WHERE folderTime >=  \'" + aa + "\' order by folderTime";
            System.Data.DataTable dt = getDataTable(str, mycon);
            if (dt.Rows.Count != 0)
            {
                aa = Convert.ToDateTime(dt.Rows[0]["folderTime"]);
                string name = CreateTable.getTableName(aa, "DTS");//获取文件将要存入的表名称
                Str = "Select wellbore.mD,wellbore.DepthH,wellbore.TVD,wellbore.inclAngle," + name + ".TM  from wellbore ," + name + "  Where wellbore.mD =(" + name + ".Depth-"+wellzero+") and " + name + ".RecordTime='" + aa + "'";
            }
            
            return Str;
        }

        public static void DrawingWell()//画图well
        {
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            ZedGraph.GraphPane myPane = MainForm.getInstance().WellTM.GraphPane;
      
            MainForm.getInstance().WellTM.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerWell);//设置节点信息显示样式
            MainForm.getInstance().WellTM.IsShowPointValues = true;//
            MainForm.getInstance().WellTM.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
            myPane.CurveList.Clear();//清除上一步画的图

            //float WellK = float.Parse(MainForm.getInstance().nn1.Text.ToString());//井套管的线宽度
            //float WellD = float.Parse(MainForm.getInstance().nn2.Text.ToString());//井套管的线的节点大小
            float WellK = 2;
            float WellD = 1;
            if (MainForm.getInstance().Diameter1.Text != "" && MainForm.getInstance().Diameter2.Text != "" && MainForm.getInstance().Diameter3.Text != "" && MainForm.getInstance().Diameter4.Text != "")
            {
                try
                {
                    float Diameter1 = float.Parse(MainForm.getInstance().Diameter1.Text.ToString());//隔水导管直径
                    float Diameter2 = float.Parse(MainForm.getInstance().Diameter2.Text.ToString());//表层套管直径
                    float Diameter3 = float.Parse(MainForm.getInstance().Diameter3.Text.ToString());//技术套管直径
                    float Diameter4 = float.Parse(MainForm.getInstance().Diameter4.Text.ToString());//油管直径
                    float b1 = Diameter1 / Diameter4;//隔水导管
                    float b2 = Diameter2 / Diameter4;
                    float b3 = Diameter3 / Diameter4;//技术套管

                }
                catch { }
               
                //SymbolType c=new SymbolType();
                //object h = SymbolType.Circle;
                //c = (SymbolType)h;
                if (MainForm.getInstance().Length1.Text != "" && MainForm.getInstance().Length2.Text != "" && MainForm.getInstance().Length3.Text != "")
                {
                    float Length1=0;
                    float Length2=0;
                    float Length3=0;
                    try
                    {
                         Length1 = float.Parse(MainForm.getInstance().Length1.Text.ToString());//隔水导管长度
                         Length2 = float.Parse(MainForm.getInstance().Length2.Text.ToString());//表层套管长度
                         Length3 = float.Parse(MainForm.getInstance().Length3.Text.ToString());//技术套管长度
                    }
                    catch
                    {
                        MessageBox.Show("长度填写不是小数或整数，图像生成会不完整！");
                    }
                    
                  
                    try
                    {
                        string Str = getWellStr();
                        if (Str == null) { MessageBox.Show("该时间点之后时间无温度数据"); }
                        else
                        {
                            string l = Str + " order by TM";
                            System.Data.DataTable dt1 = getDataTable(l, mycon);
                            float Min = float.Parse(dt1.Rows[0][4].ToString());
                            float Max = float.Parse(dt1.Rows[dt1.Rows.Count - 1][4].ToString());
                            int max = (int)(Max + 0.5);
                            int min = (int)(Min - 0.5);
                            MainForm.getInstance().label104.Text = min + "℃";
                            MainForm.getInstance().label105.Text = max + "℃";
                            getsejie(max, min);
                            System.Data.DataTable dt = getDataTable(Str, mycon);
                            DataTable dt2 = getDatatable(dt, 20);
                          //  string  vv=dt.Rows[dt.Rows.Count-1][1].ToString();
                            float v1 =float.Parse(dt.Rows[dt.Rows.Count - 1][1].ToString());
                            float v2 = float.Parse(dt.Rows[dt.Rows.Count - 1][2].ToString());
                            int Num=300;
                            if (v1 > v2)
                            {
                                Num = (int)v1 * 1 / 6;
                            }
                            else
                            {
                                Num = (int)v2 * 1 / 6;
                            }
                            myPane.X2Axis.Scale.Min = 0;
                            myPane.X2Axis.Scale.Max = Num * 8;
                            myPane.YAxis.Scale.Min = 0;
                            myPane.YAxis.Scale.Max = Num * 7;
                           // myPane.AxisChange();
                            //int Num = 300;//画图时井口的位置偏移程度（防止井身贴着Y轴）
                            DataTable dt3 = getDatatable(dt, (20 + 40));
                            DataTable dt4 = getDatatable(dt, (-22 - 40));
                            DataTable dt5 = getDatatable(dt, (20 + 80));
                            DataTable dt6 = getDatatable(dt, (-22 - 80));
                            DataTable dt7 = getDatatable(dt, (20 + 120));
                            DataTable dt8 = getDatatable(dt, (-22 - 120));

                            //DataTable dt3 = getDatatable(dt, (10 + 40 * 1));
                            //DataTable dt4 = getDatatable(dt, (-12 - 40 * 1));
                            //DataTable dt5 = getDatatable(dt, (10 + 40 * 2));
                            //DataTable dt6 = getDatatable(dt, (-12 - 40 * 2));
                            //DataTable dt7 = getDatatable(dt, (10 + 40 * 3));
                            //DataTable dt8 = getDatatable(dt, (-12 - 40 * 3));
                            myPane.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);//控件颜色填充
                            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);//画板颜色填充
                           
                            //myPane.X2Axis.Scale.Min = 0;
                            //myPane.X2Axis.Scale.Max =1500;
                            myPane.Legend.IsVisible = false;//图例是不可见的 
                            myPane.YAxis.Scale.IsReverse = true;//Y轴值翻转，图像一样翻转
                            myPane.YAxis.MinorTic.IsOpposite = false;//Y轴对面的小刻度是否可见
                            myPane.YAxis.MajorTic.IsOpposite = false;//Y轴对面的大刻度是否可见
                            myPane.X2Axis.IsVisible = true;
                            myPane.X2Axis.Title.IsVisible = true;
                            myPane.XAxis.IsVisible = false;//下方X轴消失
                            myPane.X2Axis.IsVisible = false;//下方X轴消失
                            myPane.X2Axis.MinorTic.IsOpposite = false;//X轴对面的小刻度是否可见
                            myPane.X2Axis.MajorTic.IsOpposite = false;//X轴对面的大刻度是否可见
                            myPane.YAxis.MinorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
                            myPane.YAxis.MajorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
                            myPane.X2Axis.MinorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
                            myPane.X2Axis.MajorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
                            myPane.X2Axis.MinorTic.IsInside = false;//IsInside ->> 刻度条是否要显示到坐标轴的里边。
                            myPane.X2Axis.MajorTic.IsInside = false;//IsInside ->> 刻度条是否要显示到坐标轴的里边。

                            //内外边框设置
                            MainForm.getInstance().WellTM.MasterPane.Border.IsVisible = true;

                            //为chart 设置坐标值，大小

                            int jishu =max- min  ;
                            int w = 0;
                            for (int j = 0; j < dt.Rows.Count; j++)//
                            {

                                int a = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                string name = dt.Rows[j]["TM"].ToString()+"$"+j;
                                PointPairList list1 = new PointPairList();
                                double x = float.Parse(dt.Rows[j]["DepthH"].ToString()) + Num;
                                double y = float.Parse(dt.Rows[j]["TVD"].ToString());
                                double x2 = float.Parse(dt2.Rows[j]["fb"].ToString()) + Num;
                                double y2 = float.Parse(dt2.Rows[j]["fc"].ToString());
                                list1.Add(x, y);
                                list1.Add(x2, y2);
                                Color c1; Color c2;
                                a = a - min;
                                //if (a == 69)
                                //{
                                //    int k = 0;
                                //}
                                float b = (float)a / jishu;
                                float t=(float)1/(float)3;
                                if (b <= t)
                                {
                                    w = 1;
                                    c1 = Color.Blue;
                                    c2 = Color.Cyan;
                                }
                                else if (b> t && b <= 2*t)
                                {
                                    w = 2;
                                    c1 = Color.Cyan;
                                    c2 = Color.Yellow;
                                }
                                else
                                {
                                    w = 3;
                                    c1 = Color.Yellow;
                                    c2 = Color.Red;
                                }
                                LineItem myCurve = myPane.AddCurve(name,
                                   list1, getColorTM(a, max, min, c1, c2, w), SymbolType.Circle);
                                myCurve.Line.Width = 2f;
                                myCurve.Symbol.Size = 7.0F;//线上节点的大小
                                myCurve.Symbol.Fill = new Fill(getColorTM(a, max, min, c1, c2, w));//线上节点的颜色 
                                myCurve.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画  
                                //myPane.AxisChange();
                            }
                            //技术套管上面的线
                            PointPairList list3 = new PointPairList();
                            int a3; float x3; float y3;
                            for (int j = 0; j < dt.Rows.Count-10; j++)//
                            {
                                if (j < 1000)
                                {
                                    if (j % 15 == 0 && j < Length3)
                                    {
                                        a3 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                        x3 = float.Parse(dt3.Rows[j]["fb"].ToString()) + Num + 25;
                                        y3 = float.Parse(dt3.Rows[j]["fc"].ToString());
                                        list3.Add(x3, y3);
                                    }
                                }
                                else if (800 <= j && j < 1000)
                                {
                                    if (j % 15 == 0 && j < Length3)
                                    {
                                        a3 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                        x3 = float.Parse(dt3.Rows[j]["fb"].ToString()) + Num + 25;
                                        y3 = float.Parse(dt3.Rows[j]["fc"].ToString()) - 5;
                                        list3.Add(x3, y3);
                                    }
                                }
                                else if(1000<=j&&j<1200)
                                {
                                    if (j % 15 == 0 && j < Length3)
                                    {
                                        a3 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                        x3 = float.Parse(dt3.Rows[j]["fb"].ToString()) + Num + 25;
                                        y3 = float.Parse(dt3.Rows[j]["fc"].ToString())-10;
                                        list3.Add(x3, y3);
                                    }
                                }
                                else
                                {
                                    if (j % 15 == 0 && j < Length3)
                                    {
                                        a3 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                        x3 = float.Parse(dt3.Rows[j]["fb"].ToString()) + Num + 25;
                                        y3 = float.Parse(dt3.Rows[j]["fc"].ToString()) - 15;
                                        list3.Add(x3, y3);
                                    }
                                }
                               
                            }
                            LineItem myCurve3 = myPane.AddCurve("Porsche", list3, Color.Black, SymbolType.Circle);
                            myCurve3.Line.Width = WellK;
                            myCurve3.Symbol.Size = WellD;//线上节点的大小
                            // myCurve1.Symbol.Fill = new Fill(getColor(a, max, min));//线上节点的颜色 
                            myCurve3.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画 
                            // myPane.AxisChange();
                            //技术套管下面的线
                            PointPairList list4 = new PointPairList();
                            int a4; float x4; float y4;
                            for (int j = 0; j < dt.Rows.Count; j++)//
                            {
                                if (j % 15 == 0 && j < Length3)
                                {
                                    a4 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                    x4 = float.Parse(dt4.Rows[j]["fb"].ToString()) + Num;
                                    y4 = float.Parse(dt4.Rows[j]["fc"].ToString());
                                    list4.Add(x4, y4);
                                }

                            }
                            LineItem myCurve4 = myPane.AddCurve("Porsche", list4, Color.Black, SymbolType.Circle);
                            myCurve4.Line.Width = WellK;
                            myCurve4.Symbol.Size = WellD;//线上节点的大小
                            // myCurve1.Symbol.Fill = new Fill(getColor(a, max, min));//线上节点的颜色 
                            myCurve4.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画  
                            //myPane.AxisChange();

                            //表层套管上面的线
                            PointPairList list5 = new PointPairList();
                            int a5; float x5; float y5;
                            for (int j = 0; j < dt.Rows.Count; j++)//
                            {
                                if (j % 15 == 0 && j < (Length2 + 15))
                                {
                                    a5 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                    x5 = float.Parse(dt5.Rows[j]["fb"].ToString()) + Num + 20;//+10是为了使得油管在表层套管中间
                                    y5 = float.Parse(dt5.Rows[j]["fc"].ToString());
                                    list5.Add(x5, y5);
                                }

                            }
                            LineItem myCurve5 = myPane.AddCurve("Porsche", list5, Color.Black, SymbolType.Circle);
                            myCurve5.Line.Width = WellK;
                            myCurve5.Symbol.Size = WellD;//线上节点的大小
                            // myCurve1.Symbol.Fill = new Fill(getColor(a, max, min));//线上节点的颜色 
                            myCurve5.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画 
                            //  myPane.AxisChange();
                            //表层套管下面的线
                            PointPairList list6 = new PointPairList();
                            
                            int a6; float x6; float y6;
                            for (int j = 0; j < dt.Rows.Count; j++)//
                            {
                                if (j % 15 == 0 && j < (Length2 + 15))
                                {
                                    a6 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                    x6 = float.Parse(dt6.Rows[j]["fb"].ToString()) + Num;
                                    y6 = float.Parse(dt6.Rows[j]["fc"].ToString());
                                    list6.Add(x6, y6);
                                }

                            }
                            LineItem myCurve6 = myPane.AddCurve("Porsche", list6, Color.Black, SymbolType.Circle);
                            myCurve6.Line.Width = WellK;
                            myCurve6.Symbol.Size = WellD;//线上节点的大小
                            // myCurve1.Symbol.Fill = new Fill(getColor(a, max, min));//线上节点的颜色 
                            myCurve6.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画  
                            // myPane.AxisChange();

                            //隔水导管上面的线
                            PointPairList list7 = new PointPairList();
                            int a7; float x7; float y7;
                            for (int j = 0; j < dt.Rows.Count; j++)//
                            {
                                if (j % 15 == 0 && j < (Length1 +15))
                                {
                                    a7 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                    x7 = float.Parse(dt7.Rows[j]["fb"].ToString()) + Num + 20;
                                    y7 = float.Parse(dt7.Rows[j]["fc"].ToString());
                                    //x7  = float.Parse(dt.Rows[j]["DepthH"].ToString()) + Num+200;
                                    //y7 = float.Parse(dt.Rows[j]["TVD"].ToString());
                                    list7.Add(x7, y7);
                                }

                            }
                            LineItem myCurve7 = myPane.AddCurve("Porsche", list7, Color.Black, SymbolType.Circle);
                            myCurve7.Line.Width = WellK;
                            myCurve7.Symbol.Size = WellD;//线上节点的大小
                            // myCurve1.Symbol.Fill = new Fill(getColor(a, max, min));//线上节点的颜色 
                            myCurve7.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画 
                            // myPane.AxisChange();


                            //隔水导管下面的线
                            PointPairList list8 = new PointPairList();
                            int a8; float x8; float y8;
                            for (int j = 0; j < dt.Rows.Count; j++)//
                            {
                                if (j %15 == 0 && j < (Length1 + 15))
                                {
                                    a8 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                    x8 = float.Parse(dt8.Rows[j]["fb"].ToString()) + Num;
                                    y8 = float.Parse(dt8.Rows[j]["fc"].ToString());
                                    //x8 = float.Parse(dt.Rows[j]["DepthH"].ToString()) + Num-200;
                                    //y8 = float.Parse(dt.Rows[j]["TVD"].ToString());
                                    list8.Add(x8, y8);
                                }

                            }
                            LineItem myCurve8 = myPane.AddCurve("Porsche", list8, Color.Black, SymbolType.Circle);
                            myCurve8.Line.Width = WellK;
                            myCurve8.Symbol.Size = WellD;//线上节点的大小
                            // myCurve1.Symbol.Fill = new Fill(getColor(a, max, min));//线上节点的颜色 
                            myCurve8.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画 
                            myPane.AxisChange();
                            //顶峰
                            try
                            {
                                PointPairList list9 = new PointPairList();
                                float x91;
                                float x92;
                                float y91;
                                float y92;
                                // a9 = (int)float.Parse(dt.Rows[j]["TM"].ToString());//温度取整数
                                x91 = float.Parse(dt4.Rows[int.Parse(MainForm.getInstance().dingfeng.Text)]["fb"].ToString()) + Num;
                                y91 = float.Parse(dt4.Rows[int.Parse(MainForm.getInstance().dingfeng.Text)]["fc"].ToString());
                                //x8 = float.Parse(dt.Rows[j]["DepthH"].ToString()) + Num-200;
                                //y8 = float.Parse(dt.Rows[j]["TVD"].ToString());
                                list9.Add(x91, y91);
                                x92 = float.Parse(dt3.Rows[int.Parse(MainForm.getInstance().dingfeng.Text)]["fb"].ToString()) + Num;
                                y92 = float.Parse(dt3.Rows[int.Parse(MainForm.getInstance().dingfeng.Text)]["fc"].ToString()) - 20;
                                //x8 = float.Parse(dt.Rows[j]["DepthH"].ToString()) + Num-200;
                                //y8 = float.Parse(dt.Rows[j]["TVD"].ToString());
                                list9.Add(x92, y92);
                                LineItem myCurve9 = myPane.AddCurve("Porsche", list9, Color.Blue, SymbolType.Square);
                                myCurve9.Line.Width = 5;
                                myCurve9.Symbol.Size = 5;//线上节点的大小
                                myCurve9.Symbol.Fill = new Fill(Color.Blue);//线上节点的颜色 
                                myCurve9.IsX2Axis = true;  //手动改为按【X2Axis】的刻度描画 
                                myPane.AxisChange();
                            }
                            catch
                            {
                                MessageBox.Show("顶峰深度不正确");
                            }
                           
                       

                        }
                        
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("数据不正确/未导入数据/时间点无数据" + e);
                    }
                   
                    MainForm.getInstance().WellTM.Refresh();
                }
                else
                {
                    MessageBox.Show("各个套管的深度未全部填写");
                }

              
            }
            else
            {
                MessageBox.Show("各个套管的直径未全部填写");
            }
            MainForm.getInstance().groupBox8.Enabled = true;

        }
        
        //public static void getsejie(int max, int min )
        //{
        //    ZedGraph.GraphPane myPane1 = MainForm.getInstance().zedGraphControl1.GraphPane;
        //    myPane1.Title.Text = "色阶";
        //    myPane1.Title.IsVisible = false;
        //    Point topLeft1 = new Point(0, 0);
        //    Size howBig1 = new Size(60, 255);
        //    Rectangle rectangleArea = new Rectangle(topLeft1, howBig1);
        //    myPane1.Chart.Rect = rectangleArea;
        //    MainForm.getInstance().zedGraphControl1.MasterPane.Border.IsVisible = false;
        //    myPane1.Legend.IsVisible = false;//图例是不可见的 
        //    myPane1.Border.IsVisible = false;//外边框消失
        //    myPane1.Chart.Border.IsVisible = false;//边框是否可见
        //    myPane1.YAxis.Scale.IsReverse = true;//Y轴值翻转，图像一样翻转

        //    myPane1.X2Axis.IsVisible = false;//上方X轴显示
        //    myPane1.XAxis.IsVisible = false;//下方X轴消失
        //    myPane1.Y2Axis.IsVisible = false;//上方Y轴显示
        //    myPane1.YAxis.IsVisible = false;//下方Y轴消失
        //    myPane1.YAxis.Scale.IsReverse = true;
        //    int jishu = System.Math.Abs(min) + System.Math.Abs(max);
        //    for (int j = 0; j <jishu-1; j++)
        //    {
        //        int a = j +min;
        //        PointPairList list1 = new PointPairList();
        //        for (int i = 0; i <10; i++)
        //        {
        //            double x = i;
        //            double y1 = j;
        //            list1.Add(x, y1);
        //        }
        //        LineItem myCurve = myPane1.AddCurve("Porsche",
        //           list1, getColor(a, max, min), SymbolType.Diamond);
        //        myCurve.Line.Width = 20f;
        //        myCurve.Symbol.Size = 10.0F;//线上节点的大小
        //        myPane1.AxisChange();
          
        //    }
        //    MainForm.getInstance().zedGraphControl1.Refresh();
        //}
        public static void getsejie(int max, int min)
        {
            ZedGraph.GraphPane myPane1 = MainForm.getInstance().zedGraphControl1.GraphPane;
            myPane1.CurveList.Clear();//清除上一步画的图
            myPane1.Title.Text = "色阶";
            myPane1.Title.IsVisible = false;
            Point topLeft1 = new Point(0, 0);
            Size howBig1 = new Size(44, 165);
            Rectangle rectangleArea = new Rectangle(topLeft1, howBig1);
            myPane1.Chart.Rect = rectangleArea;
            MainForm.getInstance().zedGraphControl1.MasterPane.Border.IsVisible = false;
            MainForm.getInstance().zedGraphControl1.IsEnableVZoom = false;//Y轴缩放
            MainForm.getInstance().zedGraphControl1.IsEnableHZoom = false;//x轴缩放
            myPane1.Legend.IsVisible = false;//图例是不可见的 
            myPane1.Border.IsVisible = false;//外边框消失
            myPane1.Chart.Border.IsVisible = false;//边框是否可见
            myPane1.YAxis.Scale.IsReverse = true;//Y轴值翻转，图像一样翻转

            myPane1.X2Axis.IsVisible = false;//上方X轴显示
            myPane1.XAxis.IsVisible = false;//下方X轴消失
            myPane1.Y2Axis.IsVisible = false;//上方Y轴显示
            myPane1.YAxis.IsVisible = false;//下方Y轴消失
            myPane1.YAxis.Scale.IsReverse = true;

            //12-14 将max.min 写死，不在使用,max he 
            int jishu = max-min;
            for (int j = 0; j < jishu - 1; j++)
            {
               
                PointPairList list1 = new PointPairList();
                for (int i = 0; i < 10; i++)
                {
                    double x = i;
                    double y1 = j;
                    list1.Add(x, y1);
                }
                LineItem myCurve = myPane1.AddCurve("Porsche",
                   list1, getColor(j, max, min, Color.Blue, Color.Cyan), SymbolType.None);
                myCurve.Line.Width = 20f;
                //  myCurve.Symbol.Size = 10.0F;//线上节点的大小
                myPane1.AxisChange();

            }
            for (int j = 0; j < jishu - 1; j++)
            {
              
                PointPairList list1 = new PointPairList();
                for (int i = 0; i < 10; i++)
                {
                    double x = i;
                    double y1 = j + jishu;
                    list1.Add(x, y1);
                }
                LineItem myCurve = myPane1.AddCurve("Porsche",
                   list1, getColor(j, max, min, Color.Cyan, Color.Yellow), SymbolType.None);
                myCurve.Line.Width = 20f;
                //  myCurve.Symbol.Size = 10.0F;//线上节点的大小
                myPane1.AxisChange();

            }
            for (int j = 0; j < jishu - 1; j++)
            {
             
                PointPairList list1 = new PointPairList();
                for (int i = 0; i < 10; i++)
                {
                    double x = i;
                    double y1 = j + jishu  * 2;
                    list1.Add(x, y1);
                }
                LineItem myCurve = myPane1.AddCurve("Porsche",
                   list1, getColor(j, max, min, Color.Yellow, Color.Red), SymbolType.None);
                myCurve.Line.Width = 20f;
                //  myCurve.Symbol.Size = 10.0F;//线上节点的大小
                myPane1.AxisChange();

            }
            MainForm.getInstance().zedGraphControl1.Refresh();
        }
        //public static Color getColor1(int a, int max, int min)//获取颜色——管柱图均匀色阶
        //{
        //    //1度代表一个颜色
        //    int jishu = System.Math.Abs(min) + System.Math.Abs(max);
        //    Color c;
        //    int R, G, B;
        //    Color c1 = Color.Blue;
        //    //Color c3 = Color.Yellow;
        //    Color c4 = Color.Yellow;
        //    Color c2 = Color.Red;
        //    a = a - min;
        //    if (a < jishu / 2)
        //    {
        //        R = c1.R + (c4.R - c1.R) * a / jishu;
        //        G = c1.G + (c4.G - c1.G) * a / jishu;
        //        B = c1.B + (c4.B - c1.B) * a / jishu;
        //    }
        //    else
        //    {
        //        R = c4.R + (c2.R - c4.R) * a / jishu;
        //        G = c4.G + (c2.G - c4.G) * a / jishu;
        //        B = c4.B + (c2.B - c4.B) * a / jishu;
        //    }

        //    if (R < 0 || G < 0 || B < 0)
        //    {
        //        MessageBox.Show("R:" + R + "R:" + G + "B :" + B + "a: " + a + "jishu: " + jishu);
        //    }
        //    c = Color.FromArgb(R, G, B);

        //    return c;
        //}

        public static Color getColor(int a, int max, int min, Color c1, Color c2)//获取颜色——管柱图均匀色阶
        {
            //1度代表一个颜色
            int jishu = System.Math.Abs(min) + System.Math.Abs(max);
            Color c;
            int R, G, B;

            R = c1.R + (c2.R - c1.R) * a / jishu;
            G = c1.G + (c2.G - c1.G) * a / jishu;
            B = c1.B + (c2.B - c1.B) * a / jishu;
            if (R < 0 || G < 0 || B < 0)
            {
                MessageBox.Show("R:" + R + "R:" + G + "B :" + B + "a: " + a + "jishu: " + jishu);
            }
            c = Color.FromArgb(R, G, B);

            return c;
        }
        public static Color getColorTM(int a, int max, int min, Color c1, Color c2,int  w)//获取颜色——管柱图均匀色阶
        {
            //1度代表一个颜色
            int jishu = max - min;
            Color c;
            int R, G, B;
            if (w == 1)
            {
                R = c1.R + (c2.R - c1.R) * a * 3 / jishu;
                G = c1.G + (c2.G - c1.G) * a * 3 / jishu;
                B = c1.B + (c2.B - c1.B) * a * 3 / jishu;
            }
            else if (w == 2)
            {
                R = c1.R + (c2.R - c1.R) * (a * 3 - jishu) / jishu;
                G = c1.G + (c2.G - c1.G) * (a * 3 - jishu) / jishu;
                B = c1.B + (c2.B - c1.B) * (a * 3 - jishu) / jishu;
            }
            else
            {
                R = c1.R + (c2.R - c1.R) * (a * 3 - 2*jishu) / jishu;
                G = c1.G + (c2.G - c1.G) * (a * 3 - 2*jishu) / jishu;
                B = c1.B + (c2.B - c1.B) * (a * 3-2*jishu) / jishu;
            }
            if (a > 51)
            {
               // MessageBox.Show("R:" + R + "R:" + G + "B :" + B + "a: " + a + "jishu: " + jishu);
            }
            if (R < 0 || G < 0 || B < 0)
            {
                MessageBox.Show("R:" + R + "R:" + G + "B :" + B + "a: " + a + "jishu: " + jishu);
            }
            c = Color.FromArgb(R, G, B);

            return c;
        }
        public static DataTable getDatatable(DataTable dt, float num)
        {
            DataTable table = new DataTable();
            table.Columns.Add("fa");
            table.Columns.Add("fb");
            table.Columns.Add("fc");
            float x, y, x1, y1, C;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                x =float.Parse( dt.Rows[i][1].ToString());
                y = float.Parse(dt.Rows[i][2].ToString());
                C = float.Parse(dt.Rows[i][3].ToString());
                y1 = (float)(y - num * System.Math.Abs(Math.Sin(C * Math.PI / 180)));
                x1 = (float)(x + num * System.Math.Abs(Math.Cos(C * Math.PI / 180)));
                DataRow dr = table.NewRow();//创建数据行
                dr["fa"] = int.Parse(dt.Rows[i][0].ToString());
                dr["fb"] = x1;
                dr["fc"] = y1;
               // dr["fc"] = str3;

                table.Rows.Add(dr);//将创建的数据行添加到table中
            }
                return table;
        }

    }

}
