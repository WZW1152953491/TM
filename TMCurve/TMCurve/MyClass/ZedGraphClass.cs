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
using System.Diagnostics;

namespace TMCurve.MyClass
{
    //对于ZedGraph的一些公共设置
    class ZedGraphClass:MyClass
    {

        //汉化ZedGraph的右键菜单

        public static void MyContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            int a = menuStrip.Items.Count;
            if (a < 11)//防止菜单重复添加
            {
                menuStrip.Items.Add("允许全局缩放");
                menuStrip.Items.Add("仅允许X轴缩放");
               // menuStrip.Items.Add("调换Time曲线坐标轴");
                menuStrip.Items.Add("导出");
                menuStrip.Items.Add("属性设置");
            }
            menuStrip.Items[8].Click += new EventHandler(tollgeXY);
            menuStrip.Items[9].Click += new EventHandler(tollgeX);
           // menuStrip.Items[10].Click += new EventHandler(exchange);
            menuStrip.Items[10].Click += new EventHandler(dataexport);
            menuStrip.Items[11].Click += new EventHandler(shuxingshezhi);
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                switch (item.Name)
                {
                    case "copied_to_clip":
                        item.Text = @"复制到剪贴板";
                        break;
                    case "copy":
                        item.Text = @"复制";
                        break;
                    case "page_setup":
                        item.Text = @"页面设置...";
                        break;
                    case "print":
                        item.Text = @"打印...";
                        break;
                    case "save_as":
                        item.Text = @"另存图表...";
                        break;
                    case "set_default":
                        item.Text = @"恢复默认大小";
                        item.Visible = false;
                        break;
                    case "show_val":
                        item.Text = @"显示节点数值";
                        break;
                    case "title_def":
                        item.Text = @"标题";
                        break;
                    case "undo_all":
                        item.Text = @"还原缩放";
                        break;

                    case "unpan":
                        item.Text = @"还原移动";
                        break;

                    case "unzoom":
                        item.Text = @"还原上一步缩放";
                        break;

                    case "x_title_def":
                        item.Text = @"X 轴";
                        break;
                    case "y_title_def":
                        item.Text = @"Y 轴";
                        break;
                }
            }
        }
        public static  void shuxingshezhi(object sender, EventArgs e)
        {
            shuxing f = new shuxing();
            //f.StartPosition = FormStartPosition.CenterScreen;//窗体位置在屏幕中间
            f.StartPosition = FormStartPosition.CenterParent;//窗体在其父窗口中间
            //  f.StartPosition = FormStartPosition.Manual;//窗体在有其空间的Location属性而定
            //  f.StartPosition =FormStartPosition.WindowsDefaultBounds;//窗体位置由Windows默认位置决定，窗体大小也是Windows默认大小
            //  f.StartPosition =FormStartPosition.WindowsDefaultLocation//窗体位置是Windows默认，大小在窗体大小中确定
            // f.Owner = this;
            //f.Txt = "aaaa";
            f.ShowDialog();//此时不能操作后边的窗体

        }
      
        public static void tollgeXY(object sender, EventArgs e)
        {
          //  string a = MainForm.getInstance().label1.Text;

            if (MainForm.getInstance().label1.Text == "0")
            {
                MainForm.getInstance().label1.Text = "1";    
            } 
        }
      
        public static void tollgeX(object sender, EventArgs e)
        {
            if (MainForm.getInstance().label1.Text == "1")
            {

                MainForm.getInstance().label1.Text = "0";
            }
        }
        public static void exchange(object sender, EventArgs e)
        {
            ZedGraph.GraphPane gp = MainForm.getInstance().zgcTime.GraphPane;
            if (MainForm.getInstance().exchange.Text == "0")
            {
                
                MainForm.getInstance().exchange.Text = "1";
                gp.XAxis.Title.Text = "温度"; //X轴名称;
                gp.YAxis.Title.Text = "深度"; //Y轴名称
            }
            else
            {
                MainForm.getInstance().exchange.Text = "0";
                gp.XAxis.Title.Text = "深度"; //X轴名称;
                gp.YAxis.Title.Text = "温度"; //Y轴名称
            }

        }
        public static System.Data.DataTable getDepcdsql(MySqlConnection mycon)//的到菜单导出SQL
        {
            //获取井深
            float wellzero = ZedGraphClass.getWellZero();
            DataTable dtValue = new DataTable();
            System.Data.DataTable dt = new DataTable();
             string SQLstr =null;//获取部分SQL语句
             string strSql = null;//获取全部的SQL
            if (MainForm.getInstance().tabControl1.SelectedIndex == 0)//如果是DTSDep页面
            {
                string DsingleDepth = MainForm.getInstance().DsingleDepth.Text;
                DateTime DintervalTime1 = MainForm.getInstance().DintervalTime1.Value;
                DateTime DintervalTime2 = MainForm.getInstance().DintervalTime2.Value;
                DataTable tableName = drawingDTSDep.getTNameTable(DintervalTime1, DintervalTime2);//获取需要使用的表名称
                SQLstr  = drawingDTSDep.getSQLstr();
                if (SQLstr != null) {
                    ArrayList h = MyDataTable.getDepth(wellzero, DsingleDepth);
                    string DepString;
                    if (h.Count != 0)
                    {
                        DepString = "\'" + h[0].ToString() + "\'";
                        for (int i = 1; i < h.Count; i++)
                        {
                            DepString = DepString + ",\'" + h[i].ToString() + "\'";

                        }
                        for (int j = 0; j < tableName.Rows.Count; j++)
                        {
                            strSql = null;
                            strSql = "SELECT Depth,RecordTime,TM from " + tableName.Rows[j][0] + " " + SQLstr + " and  Depth  in (" + DepString + ") ORDER BY RecordTime";
                            dt = getDataTable(strSql, mycon);
                            dtValue.Merge(dt);
                        }

                        return dtValue;               
                    }
                    else
                    {
                        return dtValue;
                    }
                }
                else
                {
                    return null;
                }
            }
         else if (MainForm.getInstance().tabControl1.SelectedIndex == 1)//如果是DTSTime页面
            {
                SQLstr = drawingzgcTime.getstrT(wellzero);
                if (SQLstr != null)
                {
                    DataTable SQLTableTime = drawingzgcTime.getTime(mycon);

                    if (SQLTableTime.Rows.Count != 0)
                    {
                      
                        for (int i =0; i < SQLTableTime.Rows.Count; i++)
                        {
                            strSql = null;
                            strSql = "SELECT Depth,RecordTime,TM from " + SQLTableTime.Rows[i]["folderTable"] +" "+ SQLstr + " and  RecordTime = '" + SQLTableTime.Rows[i]["folderTime"] + "'ORDER BY RecordTime";
                            dt = getDataTable(strSql, mycon);
                            dtValue.Merge(dt);
                         
                        }      
                        return dtValue;
                    }
                    else
                    {
                        return null;
                    }
                }

                else
                {
                    return null;
                }
             
            }
            else if (MainForm.getInstance().tabControl1.SelectedIndex == 2)//如果是DTSTime页面
            {
                string GratDepth = MainForm.getInstance().textBox3.Text;
                DateTime GratDepTime1 = MainForm.getInstance().GratDepTime1.Value;
                DateTime GratDepTime2 = MainForm.getInstance().GratDepTime2.Value;
                DataTable tableName = drawingGDep.getTNameTable(GratDepTime1, GratDepTime2);//获取需要使用的表名称
                SQLstr = drawingGDep.getstr();//获取str
                if (SQLstr != null)
                {
                    ArrayList h = MyDataTable.getDepth(wellzero, GratDepth);
                    h = getNewDepth(h);
                   // string str1 = null, Strnum = null;
                    string DepString;
                    if (h.Count != 0)
                    {
                        DepString = "\'" + h[0].ToString() + "\'";
                        for (int i = 1; i < h.Count; i++)
                        {
                            DepString = DepString + ",\'" + h[i].ToString() + "\'";

                        }
                        // str1 = str + " and  Depth in (" + DepString + ")";
                        for (int j = 0; j < tableName.Rows.Count; j++)
                        {
                            strSql = null;
                            strSql = "SELECT Depth,RecordTime,TM from " + tableName.Rows[j][0] + " " + SQLstr + " and  Depth  in (" + DepString + ") ORDER BY RecordTime";
                            dt = getDataTable(strSql, mycon);
                            dtValue.Merge(dt);
                        }

                        return dtValue;
                    }
                    else
                    {
                       
                        return null;
                    }
                }

                else
                {
                   
                    return null;
                }
            }
            else if (MainForm.getInstance().tabControl1.SelectedIndex == 3)//如果是DTSTime页面
            {

                DataTable SQLTableTime = drawingGTime.getGTime(mycon);
                if (SQLTableTime.Rows.Count != 0)
                {
                    for (int i = 0; i < SQLTableTime.Rows.Count; i++)
                    {
                        strSql = null;
                        strSql = "SELECT Depth,RecordTime,TM from " + SQLTableTime.Rows[i]["folderTable"] + " where  RecordTime = '" + SQLTableTime.Rows[i]["folderTime"] + "'ORDER BY RecordTime";
                        dt = getDataTable(strSql, mycon);
                        dtValue.Merge(dt);
                    }
                    return dtValue;
                }
                else
                {
                    return null;
                }  
            }
            else
            {
                //MessageBox.Show("此图形数据无导出功能"); 

                return dtValue;
 
            }
         
          
        }
        public static void dataExport()
        {
            //MySqlConnection mycon;
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            int exportNum = 0;
            if (MainForm.getInstance().tabControl1.SelectedIndex == 0 || MainForm.getInstance().tabControl1.SelectedIndex == 1)
            {
                exportNum = 1;
            }
            DataTable dt = getDepcdsql(mycon);
            if (dt != null)
            {
                try
                {
                    string fileName = null;
                    if (dt.Rows.Count != 0)
                    {

                        string saveFileName = "";
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "csv";
                        saveDialog.Filter = "Excel文件|*.csv";
                        DateTime startTime = DateTime.Now;//计算程序运行时间
                        saveDialog.FileName = fileName;

                        ////打开的文件选择对话框上的标题  
                        saveDialog.Title = "请填写文件名和选择文件路径";
                        ////设置文件类型  
                        //saveDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                        ////设置默认文件类型显示顺序  
                        //saveDialog.FilterIndex = 1;
                        ////保存对话框是否记忆上次打开的目录  
                        //saveDialog.RestoreDirectory = true;
                        ////设置是否允许多选  
                        //saveDialog.Multiselect = false;
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            saveFileName = saveDialog.FileName;
                            // SaveCSV(dt, saveFileName);
                            TimeSpan ts = DateTime.Now - startTime;
                            try
                            {
                                export.SaveCSV(dt, saveFileName, exportNum);
                                MessageBox.Show("文件： " + saveFileName + "保存成功，总共花费时间:" + ts.ToString(), "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("导出失败" + ex);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("没有数据");
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("导出失败" + ex);
                }
            }
            else
            {
                MessageBox.Show("数据为空！");
            }
            mycon.Close();
            mycon.Dispose();
          //  MainForm.threadImport.Abort();

        }
        public static void dataexport(object sender, EventArgs e)//菜单导出
        {
            if (MainForm.threadImport == null)//判断线程是否为空，为空则直接执行任务
            {
                MainForm.threadImport = new Thread(dataExport);
                MainForm.threadImport.SetApartmentState(ApartmentState.STA);
                MainForm.threadImport.Start();
            }
            else 
            {
                if (MainForm.threadImport.IsAlive == true)//当线程为不为时，判断是否在运行，在运行则提示线程依旧在工作
                {
                    MessageBox.Show("已有导出任务，请稍后！");
                }
                else
                {
                    MainForm.threadImport = new Thread(dataExport);
                    MainForm.threadImport.SetApartmentState(ApartmentState.STA);
                    MainForm.threadImport.Start();
                }
            }
         
               
         
        }
        //获取随机色
        public static System.Drawing.Color GetColor(int k)
        {
            //Random randomNum_1 = new Random(Guid.NewGuid().GetHashCode());
            //System.Threading.Thread.Sleep(randomNum_1.Next(1));
            //int int_Red = randomNum_1.Next(255);

            //Random randomNum_2 = new Random((int)DateTime.Now.Ticks);
            //int int_Green = randomNum_2.Next(255);

            //Random randomNum_3 = new Random(Guid.NewGuid().GetHashCode());

            //int int_Blue = randomNum_3.Next(255);
            //int_Blue = (int_Red + int_Green > 380) ? int_Red + int_Green - 380 : int_Blue;
            //int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            //return GetDarkerColor(System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue));
            //Color sourceColor = Color.Blue;
            //Color destColor = Color.Red;
            //int redSpace = destColor.R - sourceColor.R;
            //int greenSpace = destColor.G - sourceColor.G;
            //int blueSpace = destColor.B - sourceColor.B;
            //Color[] Co = new Color{Color.Beige,Color.Bisque};
            ////Color vColor = Color.FromArgb(sourceColor.R + (int)((double)k*50 / 200 * redSpace), sourceColor.G + (int)((double)k*50 / 200 * greenSpace),sourceColor.B + (int)((double)k*50 / 200 * blueSpace) );
            //Color vColor = Color.FromArgb(k * 25, 255 - k * 25, 100 + k * 10);
           Color vColor;
            switch(k){
                case 0: vColor = Color.Red; break;
                case 1: vColor = Color.Sienna; break;
                case 2: vColor = Color.LightSkyBlue; break;
                case 3: vColor = Color.LightSeaGreen; break;
                case 4: vColor = Color.Cyan; break;
                case 5: vColor = Color.LightSalmon; break;
                case 6: vColor = Color.Magenta; break;
                case 7: vColor = Color.Orange; break;
                case 8: vColor = Color.BlueViolet; break;
                case 9: vColor = Color.Crimson; break;
                case 10: vColor = Color.Gold; break;
                case 11: vColor = Color.Blue; break;
                case 12: vColor = Color.Lime; break;
                case 13: vColor = Color.LawnGreen; break;
                default: vColor=getColor(); break;
            }
            return vColor;
        }
        private static Color getColor()
        {
            Random randomNum_1 = new Random(Guid.NewGuid().GetHashCode());
            System.Threading.Thread.Sleep(randomNum_1.Next(1));
            int int_Red = randomNum_1.Next(255);

            Random randomNum_2 = new Random((int)DateTime.Now.Ticks);
            int int_Green = randomNum_2.Next(255);

            Random randomNum_3 = new Random(Guid.NewGuid().GetHashCode());
            int int_Blue = randomNum_3.Next(255);
            int_Blue = (int_Red + int_Green > 380) ? int_Red + int_Green - 380 : int_Blue;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return GetDarkerColor(System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue));
            
        }

        //获取加深颜色
        private static Color GetDarkerColor(Color color)
        {
            const int max = 255;
            int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255); //还可以根据需要调整此处的值


            int r = Math.Abs(Math.Min(color.R - increase, max));
            int g = Math.Abs(Math.Min(color.G - increase, max));
            int b = Math.Abs(Math.Min(color.B - increase, max));


            return Color.FromArgb(r, g, b);
        }
        //时间温度曲线节点气泡显示格式
        public static string MyPointValueHandlerDep(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            String LineName = Convert.ToString(curve.Label.Text);
            PointPair pt = curve[iPt];
            string Y = pt.Y.ToString().Substring(0, pt.Y.ToString().IndexOf(".") + 2);
            return  "Linename:" + LineName+" \nT:" + Y + "°C \nTime:" + DateTime.FromOADate(pt.X) ;
        }
        public static string MyPointValueHandlerWell(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            String LineName = Convert.ToString(curve.Label.Text);
            PointPair pt = curve[iPt];
            string Y = pt.Y.ToString().Substring(0, pt.Y.ToString().IndexOf(".") + 2); 
            if (LineName != "Porsche")
            {
                string[] arrTemp = LineName.Split('$');
                String intNumber = arrTemp[0].Substring(0, arrTemp[0].IndexOf(".")+2);
                return "T:" + intNumber + "℃ \nTVD:" + Y + "m \nMD:" + arrTemp[1] + "m";
            }
            return "TVD:" + Y + "m ";
        }
        //温度深度曲线节点气泡显示格式
        public static string MyPointValueHandlerTime(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair pt = curve[iPt];
            String LineName = Convert.ToString(curve.Label.Text);
            string Y = pt.Y.ToString().Substring(0, pt.Y.ToString().IndexOf(".") + 2);
            return "Linename:" + LineName+" \nT:" + Y + "°C \nMD:" + pt.X+"m"  ;
            //return "深度: " + pt.X.ToString("f1") + "/m \n温度是 " + pt.Y.ToString("f1") + "";
        }
        //时间数组去重
        //public static ArrayList getNewTime(ArrayList Depth)
        //{
        //    ArrayList ADepth = new ArrayList();
        //    String[] Dep = new String[Depth.Count];
        //    for (int i = 0; i < Depth.Count; i++)//将单项深度导入int数组，看是否正确
        //    {
        //        String n = Convert.ToString(Depth[i]);
        //        int id = Array.IndexOf(Dep, n);
        //        if (id == -1)  //不存在
        //        {
        //            Dep[i] = n;
        //            ADepth.Add(n);
        //        }
        //    }
        //    return ADepth;
        //}
        //深度数组去重
        public static ArrayList getNewDepth(ArrayList Depth)
        {
            ArrayList ADepth = new ArrayList();
            int[] Dep = new int[Depth.Count];
            for (int i = 0; i < Depth.Count; i++)//将单项深度导入int数组，看是否正确
            {
                int n = Convert.ToInt32(Depth[i]);
                int id = Array.IndexOf(Dep, n);
                if (id == -1)  //不存在
                {
                    Dep[i] = n;
                    ADepth.Add(n);
                }
            }
            return ADepth;
        }
        public static long getL(DateTime dt)//获取时间戳
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            //long timeStamp = (long)(dt - dt1).TotalSeconds; // 相差秒数
            long timeStamp = (long)(dt - startTime).TotalMilliseconds; // 相差秒数
            return timeStamp;
        }
        public static float getWellZero()
        {
            float wellzero = 0;
            if (shuxing.getInstance().Wellzero.Text != "")
            {
                wellzero = drawAttribute.wellZero;
            }
            else
            {
                wellzero = 150;
            }
            return wellzero;
        }
        public static DateTime getDT(long l)//时间戳转换为时间
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt11 = startTime.AddMilliseconds(l);
            return dt11;
        }
        //ZedGraph 格式的一部分基础设定
        public static void stile(GraphPane gp)
        {
            //MainForm.getInstance().zgcTime.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerTime);//设置节点信息显示样式
            //MainForm.getInstance().zgcTime.IsShowHScrollBar = true;//横向滚动条
            //MainForm.getInstance().zgcTime.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
            //MainForm.getInstance().zgcTime.IsShowPointValues = true;//是否显示节点值
            //MainForm.getInstance().zgcTime.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。


            //面板以及滑板的颜色，渐变。
            //gp.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);//控件颜色填充
            //gp.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);//画板颜色填充

            //为chart 设置坐标值，大小
            //PointPairList list2 = new PointPairList();
            //Point topLeft = new Point(140, 100);//画板开始坐标
            //Size howBig = new Size(700, 500);//画板大小
            //Rectangle rectangleArea = new Rectangle(topLeft, howBig);
            //gp.Chart.Rect = rectangleArea;//设置画板大小，坐标

            //myPane.Legend.IsVisible = false;//图例是不可见的 
            //myPane.YAxis.Scale.IsReverse = true;//Y轴值从大到小，翻转，图像一样翻转
            //myPane.YAxis.MinorTic.IsOpposite = false;//Y轴对面的小刻度是否可见，即是Y2刻度
            //myPane.YAxis.MajorTic.IsOpposite = false;//Y轴对面的大刻度是否可见，即是Y2刻度

            // myPane.X2Axis.IsVisible = true;//X2可见
            //myPane.X2Axis.Scale.IsVisible = false;//上方X2轴刻度值消失
            // myPane.X2Axis.Title.IsVisible = true;
            //myPane.XAxis.IsVisible = false;//下方X轴消失

            //X2刻度值，刻度条消失与myPane.X2Axis.IsVisible = false;差一个标题是否显示
            //gp.X2Axis.Title.IsVisible = false;//X2标题不显示
            //myPane.X2Axis.MinorTic.IsOpposite = false;//X轴对面的小刻度是否可见
            //myPane.X2Axis.MajorTic.IsOpposite = false;//X轴对面的大刻度是否可见
            //myPane.X2Axis.MinorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            //myPane.X2Axis.MajorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            //myPane.X2Axis.MinorTic.IsInside = false;//IsInside ->> 刻度条是否要显示到坐标轴的里边。
            //myPane.X2Axis.MajorTic.IsInside = false;//IsInside ->> 刻度条是否要显示到坐标轴的里边。


            //内外边框设置
            //Form1.getInstance().zedGraphControl1.BorderStyle = BorderStyle.FixedSingle;//外边框的样式
            // myPane.Border.IsVisible = false;//外边框消失
            //myPane.Chart.Border.IsVisible = false;//nei边框是否可见
            //myPane.Legend.Gap = 12f;//图例与图表之间的距离
            //Form1.getInstance().zedGraphControl1.MasterPane.Border.IsVisible = true;
            //myPane.Chart.Border.Width = 20f;//内边框宽度

            //滚动轴设置
            //MainForm.getInstance().zgcTime.IsAutoScrollRange = false;
            //MainForm.getInstance().zgcTime.ScrollMaxX = DintervalDepth4 + 2;//最大。DintervalDepth4是读取的一个值
            //MainForm.getInstance().zgcTime.ScrollMinX = DintervalDepth3 - 2;//最小

            //最值问题。
            //gp.YAxis.Scale.Min = int.Parse(MainForm.getInstance().DintervalTM3.Text);
            //gp.YAxis.Scale.Max = int.Parse(MainForm.getInstance().DintervalTM4.Text);

            //缩放问题
            //MainForm.getInstance().zgcTime.IsEnableVZoom = false;//禁止Y轴缩放
            //MainForm.getInstance().zgcTime.IsEnableHZoom = true;//x轴缩放

            //清除上一步画的图
            gp.CurveList.Clear();
            gp.GraphObjList.Clear();

            //设置标题名称
            //gp.X2Axis.Title.Text = "水平长度";
            //gp.YAxis.Title.Text = "深度"; //Y軸的名稱
            //让标题水平显示
            //gp.YAxis.Title.FontSpec.Angle = 90; //标题的偏转角度

            //标题大小字体
            gp.Title.FontSpec.Size = 10f;//图表轴名称大小
            gp.Title.FontSpec.Family = "微软雅黑";
            gp.XAxis.Title.FontSpec.Size = 10f;//大小
            gp.XAxis.Title.FontSpec.Family = "微软雅黑";
            gp.YAxis.Title.FontSpec.Size = 10f;//大小
            gp.YAxis.Title.FontSpec.Family = "微软雅黑";

            //刻度大小字体
            gp.XAxis.Scale.FontSpec.Size = 9.0f;//X轴刻度
            gp.XAxis.Scale.FontSpec.Family = "微软雅黑";
            gp.YAxis.Scale.FontSpec.Size = 9.0f;//Y轴刻度
            gp.YAxis.Scale.FontSpec.Family = "微软雅黑";

            //图例大小字体
            gp.Legend.FontSpec.Size = 8.0f;//图例字体的大小
            gp.Legend.FontSpec.Family = "微软雅黑";

            //网格辅助线及颜色
            gp.XAxis.MajorGrid.IsVisible = true;//水平辅助线
            gp.XAxis.MajorGrid.Color = Color.Green;
            //gp.XAxis.MinorGrid.IsVisible = true;//水平辅助线,小刻度
            //gp.XAxis.MinorGrid.Color = Color.Green;
            //gp.YAxis.MinorGrid.IsVisible = true;//垂直辅助线，小刻度
            //gp.YAxis.MinorGrid.Color = Color.Green;
            gp.YAxis.MajorGrid.IsVisible = true;//水平辅助线
            gp.YAxis.MajorGrid.Color = Color.Green;


            //刻度设置
            gp.X2Axis.MinorTic.IsOpposite = false;//X轴对面的小刻度是否可见
            gp.X2Axis.MajorTic.IsOpposite = true;//X轴对面的大刻度是否可见
            gp.Y2Axis.MinorTic.IsOpposite = false;//X轴对面的小刻度是否可见
            gp.Y2Axis.MajorTic.IsOpposite = true;//X轴对面的大刻度是否可见
            gp.YAxis.MinorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            gp.YAxis.MajorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            gp.XAxis.MinorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            gp.XAxis.MajorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            gp.X2Axis.MinorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            gp.X2Axis.MajorTic.IsOutside = false;//IsOutside ->> 刻度条是否要显示到坐标轴的外边。
            gp.YAxis.MajorGrid.IsZeroLine = false;//Y轴等于0的实线消失


            gp.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);//控件颜色填充
            gp.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);//画板颜色填充
            //gp.X2Axis.MinorTic.IsInside = false;//IsInside ->> 刻度条是否要显示到坐标轴的里边。
            //gp.X2Axis.MajorTic.IsInside = false;//IsInside ->> 刻度条是否要显示到坐标轴的里边。
    
            // gp.Legend.Position = ZedGraph.LegendPos.BottomFlushLeft;//图例的位置
            //gp.XAxis.Scale.MinorStepAuto = true;
            //gp.XAxis.Scale.MajorStepAuto = true;
            //gp.XAxis.Type = AxisType.Date;
            //gp.Title.FontSpec.FontColor = Color.Blue;//图表轴名称颜色
            //gp.XAxis.Scale.MinorStep = 30;
            //gp.XAxis.Scale.MajorUnit = DateUnit.Millisecond;
            //gp.XAxis.Scale.Format = "yyyy-mm-dd HH:mm:ss";//横轴格式    
            //gp.XAxis.Scale.MajorStepAuto = true;
            //gp.XAxis.Scale.MinorStepAuto = true;
            //gp.XAxis.Scale.FontSpec.FontColor = Color.Black;//X轴刻度颜色
            // gp.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, Color.ForestGreen), 45.0F);//面板的颜色
            // gp.Legend.Position = ZedGraph.LegendPos.BottomFlushLeft;//图例的位置
            //gp.XAxis.Scale.MinorStepAuto = true;
            //gp.XAxis.Scale.MajorStepAuto = true;
            //gp.XAxis.Type = AxisType.Date;
            //gp.Title.FontSpec.FontColor = Color.Blue;//图表轴名称颜色
            //gp.XAxis.Scale.MinorStep = 30;
            //gp.XAxis.Scale.MajorUnit = DateUnit.Millisecond;
            //gp.XAxis.Scale.Format = "yyyy-mm-dd HH:mm:ss";//横轴格式    
            //gp.XAxis.Scale.MajorStepAuto = true;
            //gp.XAxis.Scale.MinorStepAuto = true;
            //gp.XAxis.Scale.FontSpec.FontColor = Color.Black;//X轴刻度颜色
            // gp.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, Color.ForestGreen), 45.0F);//面板的颜色


            //画图实例
            //for (int i = 0; i < h.Count; i++)
            //{
            //    //清除上一步画的图
            //    Thread.Sleep(1000);
            //    string Str = strTime + " and RecordTime=\'" + Convert.ToDateTime(h[i]) + "\' ORDER BY RecordTime,Depth";
            //    hn[i] = h[i].ToString();
            //    MySqlDataAdapter sda = new MySqlDataAdapter(Str, myconT);
            //    System.Data.DataTable dt = new System.Data.DataTable();
            //    sda.Fill(dt);

            //    PointPairList list1 = new PointPairList();
            //    for (int j = 0; j < dt.Rows.Count; j++)
            //    {
            //        double x = float.Parse(dt.Rows[j]["Depth"].ToString());
            //        float y = float.Parse(dt.Rows[j]["TM"].ToString());
            //        list1.Add(x, y);
            //    }
            //    if (list1.Count == 0)//如果曲线没有数据
            //    {
            //        //MessageBox.Show("曲线不存在");
            //        continue;
            //    }
            //    else
            //    {
            //        Color co = ZedGraphClass.GetColor(i);
            //        LineItem _lineitem2 = gp.AddCurve(hn[i], list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
            //        _lineitem2.Line.Width = 2.0F;//线的宽度

            //        _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
            //        _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色   
            //        _lineitem2.Line.Fill = new Fill(Color.White, Color.Red, 45f); //曲线下方的颜色填充   

            //绘制箭头，颜色，大小，七点坐标，结束坐标

            //ArrowObj myaw = new ArrowObj(Color.Red, 12F, 0F, 0F, 200F, 80F);
            //gp.GraphObjList.Add(myaw);
            //        gp.AxisChange();//坐标轴刷新
            //        MainForm.getInstance().zgcTime.Refresh();//节点刷新
            //    }

            //}
        }
        public static void clear_Time()//清除时间曲线和条件
        {
           //MainForm.getInstance().TimeList.Text = "";
           MainForm.getInstance().DintervalDepth3.Text = "";
           MainForm.getInstance().DintervalDepth4.Text = "";
           MainForm.getInstance().DintervalTM3.Text = "";
           MainForm.getInstance().DintervalTM4.Text = "";
           MainForm.getInstance().TimeList.Items.Clear();
           MainForm.getInstance().zgcTime.GraphPane.CurveList.Clear();//清除上一步画的图
           MainForm.getInstance().zgcTime.GraphPane.GraphObjList.Clear();
           MainForm.getInstance().zgcTime.Refresh();
        }
        public static void clear_Depth()//清除深度曲线和条件
        {
            //MainForm.getInstance().DintervalDepth1.Text = "";
            //MainForm.getInstance().DintervalDepth2.Text = "";
            MainForm.getInstance().DsingleDepth.Text = "";
            MainForm.getInstance().DintervalTM1.Text = "";
            MainForm.getInstance().DintervalTM2.Text = "";
         
            MainForm.getInstance().DintervalTime1.Value = System.DateTime.Now;
            MainForm.getInstance().DintervalTime2.Value = System.DateTime.Now;
            MainForm.getInstance().TimeList.Items.Clear();
         

            MainForm.getInstance().zgcDep.GraphPane.CurveList.Clear();//清除上一步画的图
            MainForm.getInstance().zgcDep.GraphPane.GraphObjList.Clear();
            MainForm.getInstance().zgcDep.Refresh();
        }
        public static void clear_GratDepth()//清除Grat深度曲线和条件
        {
            MainForm.getInstance().textBox3.Text = "";
            MainForm.getInstance().GratDepTM1.Text = "";
            MainForm.getInstance().GratDepTM2.Text = "";
            MainForm.getInstance().GratDepTime1.Value = System.DateTime.Now;
            MainForm.getInstance().GratDepTime2.Value = System.DateTime.Now;
            MainForm.getInstance().GDep.GraphPane.CurveList.Clear();//清除上一步画的图
            MainForm.getInstance().GDep.GraphPane.GraphObjList.Clear();
            MainForm.getInstance().GDep.Refresh();
        }
        public static void clear_GratTime()//清除Grat深度曲线和条件
        {
            //MainForm.getInstance().textBox3.Text = "";
            MainForm.getInstance().GratDepTM3.Text = "";
            MainForm.getInstance().GratDepTM4.Text = "";
          
            MainForm.getInstance().GTime5.Value = System.DateTime.Now;
            MainForm.getInstance().GTimeList.Items.Clear();
            MainForm.getInstance().GTime.GraphPane.CurveList.Clear();//清除上一步画的图
            MainForm.getInstance().GTime.GraphPane.GraphObjList.Clear();
            MainForm.getInstance().GTime.Refresh();
        }
        //private static void SaveCSV(System.Data.DataTable dt, string fullPath)//table数据写入csv  
        //{
           
        //    System.IO.FileInfo fi = new System.IO.FileInfo(fullPath);
        //    if (!fi.Directory.Exists)
        //    {
        //        fi.Directory.Create();
        //    }
        //    System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Create,
        //        System.IO.FileAccess.Write);
        //    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
        //    string data = "列名,";
        //    string data1;
        //    System.Data.DataTable ds=null;
        //    System.Data.DataTable dd = null ;

        //    if (MainForm.getInstance().comboBox1.Text == "深度为纵轴，时间为横轴")
        //    {
                
                 
        //         ds = export.SelectDistinct(dt, dt.Columns[1].ColumnName.ToString());
        //         dd = export.SelectDistinct(dt, dt.Columns[0].ColumnName.ToString());
        //    }
        //    else
        //    {
        //        ds = export.SelectDistinct(dt, dt.Columns[0].ColumnName.ToString());
        //        dd = export.SelectDistinct(dt, dt.Columns[1].ColumnName.ToString());
               
        //    }
            
        //    for (int i = 0; i < ds.Rows.Count; i++)//写入列名  
        //    {
        //        data1 = ds.Rows[i][0].ToString();
        //        data += "\t" + data1;//"\t"是为了导出时保留原格式
        //        if (i < ds.Rows.Count - 1)
        //        {
        //            data += ",";
        //        }
        //    }
        //    sw.WriteLine(data);

        //    //Range range = worksheet.get_Range(worksheet.Cells[2, 1], worksheet.Cells[RowCount + 1, ColCount]);
        //    //range.NumberFormat = @"yyyy-mm-dd"; //日期格式
        //    for (int i = 0; i < dd.Rows.Count; i++) //写入各行数据  
        //    {
        //        //得到row的数据
        //        string data2 = dd.Rows[i][0].ToString();
        //        data2 = "\t" + data2;
        //        DataRow[] drss = null;
        //        if (MainForm.getInstance().comboBox1.Text == "深度为纵轴，时间为横轴")
        //        {
        //            drss = dt.Select("Depth = '" + data2 + "'");//每行数据
        //        }
        //        else
        //        {
        //            drss = dt.Select("RecordTime = '" + data2 + "'");
                    
        //        }


                
        //        for (int k = 0; k < drss.Length; k++)
        //        {
        //            string b = drss[k][2].ToString();
        //            data2 += "," + "\t" + b;
        //        }
                
        //        //MainForm.getInstance().progressBar2.Value++;

        //        sw.WriteLine(data2);
        //    }
         
        //    sw.Close();
        //    fs.Close();
        //}
    }

}
