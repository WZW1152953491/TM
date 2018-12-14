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

    class drawingGTime : MyClass
    {

       
      
        //public static string strTime;
       // public static int bu = 0;//这个变量是为了让出错的时候只输出一次弹窗而设计的
        public static int num = 0;//用于控制是否循环生成图像
        public static string messageError = null;


        private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        {
            using (Graphics gc = MainForm.getInstance().GTime.CreateGraphics())
            using (Pen pen = new Pen(Color.Gray))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                RectangleF rect = MainForm.getInstance().GTime.GraphPane.Chart.Rect;
                //确保在画图区域
                if (rect.Contains(e.Location))
                {
                    MainForm.getInstance().GTime.Refresh();
                    gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
                    gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);

                }
            }

        }


        public static void DrawingGratTime()//画深度温度曲线
        {
            float wellzero = ZedGraphClass.getWellZero();
            MainForm.getInstance().groupBox4.Enabled = false;
            MainForm.getInstance().GTime.Enabled = false;
            MySqlConnection mycon = new MySqlConnection();
            mycon = getMycon();
            string button = MainForm.getInstance().number.Text;
            //ArrayList h2 = getGTime(mycon);//获取表名称列表
            DataTable SQLTableTime = getGTime(mycon);//获取时间值
                if (SQLTableTime.Rows.Count < 15 && SQLTableTime.Rows.Count > 0)//15条线之内
                {
                    ZedGraph.GraphPane gp = MainForm.getInstance().GTime.GraphPane;
                    gp.GraphObjList.Clear();
                    gp.CurveList.Clear();//清除上一步画的图
                    if (MainForm.getInstance().label1.Text == "1")
                    {
                        MainForm.getInstance().GTime.IsEnableVZoom = true;//Y轴缩放
                        MainForm.getInstance().GTime.IsEnableHZoom = true;//x轴缩放

                    }
                    else
                    {

                        MainForm.getInstance().GTime.IsEnableVZoom = false;//禁止Y轴缩放
                        MainForm.getInstance().GTime.IsEnableHZoom = true;//x轴缩放


                    }
                    MainForm.getInstance().GTime.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerTime);//设置节点信息显示样式
                    //MainForm.getInstance().GTime.IsShowHScrollBar = true;//横向滚动条
                    MainForm.getInstance().GTime.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
                    MainForm.getInstance().GTime.IsShowPointValues = true;//
                    MainForm.getInstance().GTime.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。

                    //设置XY轴的起始点

                    if (MainForm.getInstance().GratDepTM3.Text != "" && MainForm.getInstance().GratDepTM4.Text != "")
                        {
                            gp.YAxis.Scale.Min = float.Parse(MainForm.getInstance().GratDepTM3.Text);
                            gp.YAxis.Scale.Max = float.Parse(MainForm.getInstance().GratDepTM4.Text);

                        }
                        else
                        {
                            gp.YAxis.Scale.MaxAuto = true;
                            gp.YAxis.Scale.MinAuto = true;
                        }
                   
                    MainForm.getInstance().GTime.IsAutoScrollRange = false;
                   
                    //ZedGraphClass.stile(gp);//在程序打开时已经加载过了
                    //坐标轴标题格式
                    // gp.Title.Text = "LD27-2平台 A22H井 温度深度曲线"; //图表轴名称
                    //gp.XAxis.Title.Text = "深度"; //X轴名称;
                    //gp.YAxis.Title.Text = "温度"; //Y轴名称
                    


                    string[] hn = new string[SQLTableTime.Rows.Count];//图例名称
                    if (button == "two")//循环生成
                    {
                        for (int k = 0; k < 4; k++)//循环四次结束循环
                        {
                            gp.GraphObjList.Clear();
                            gp.CurveList.Clear();
                            for (int i = 0; i < SQLTableTime.Rows.Count; i++)
                            {
                                //清除上一步画的图
                                System.Threading.Thread.Sleep(1500);
                                string Str = null;
                                DataTable dtValue = new DataTable();
                                Str = "SELECT Depth,RecordTime,TM from " + SQLTableTime.Rows[i]["folderTable"] + " where  RecordTime=\'" + Convert.ToDateTime(SQLTableTime.Rows[i]["folderTime"]) + "\' ORDER BY Depth";
                                // str3 = "SELECT COUNT(*) from " + tableName.Rows[j][0] + " WHERE RecordTime between  \'" + DintervalTime1 + "\'  AND \'" + DintervalTime2 + "\'";
                                dtValue = getDataTable(Str, mycon);
                                hn[i] = SQLTableTime.Rows[i]["folderTime"].ToString();
                                PointPairList list1 = new PointPairList();
                                for (int j = 0; j < dtValue.Rows.Count; j++)
                                {
                                    double x;
                                    float y;
                                    if (num == 1)
                                    {
                                        x = float.Parse(dtValue.Rows[j]["TM"].ToString());
                                        y = float.Parse(dtValue.Rows[j]["Depth"].ToString()) - wellzero;
                                    }
                                    else
                                    {
                                        x = float.Parse(dtValue.Rows[j]["Depth"].ToString()) - wellzero;
                                        y = float.Parse(dtValue.Rows[j]["TM"].ToString());
                                    }
                                    list1.Add(x, y);
                                }
                                if (list1.Count == 0 && k == 0)//如果曲线没有数据
                                {
                                    messageError += "时间" + (MainForm.getInstance().TimeList.Items[i]).ToString() + "无数据\n";
                                    continue;
                                }
                                else
                                {
                                    Color co = ZedGraphClass.GetColor(i);
                                    LineItem _lineitem2 = gp.AddCurve(hn[i], list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
                                    _lineitem2.Line.Width = 2.0F;//线的宽度
                                    _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                    if (drawAttribute.Linenum == 2)
                                    {
                                        _lineitem2.Line.IsVisible = false;
                                    }
                                    _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色                          
                                    gp.AxisChange();
                                    MainForm.getInstance().GTime.Refresh();
                                }

                            }
                        }

                    }
                    else//生成
                    {
                        for (int i = 0; i < SQLTableTime.Rows.Count; i++)
                        {
                            string Str = null;
                            DataTable dtValue = new DataTable();
                            Str = "SELECT Depth,RecordTime,TM from " + SQLTableTime.Rows[i]["folderTable"] + " where  RecordTime=\'" + Convert.ToDateTime(SQLTableTime.Rows[i]["folderTime"]) + "\' ORDER BY Depth";
                            // str3 = "SELECT COUNT(*) from " + tableName.Rows[j][0] + " WHERE RecordTime between  \'" + DintervalTime1 + "\'  AND \'" + DintervalTime2 + "\'";
                            dtValue = getDataTable(Str, mycon);
                            hn[i] = SQLTableTime.Rows[i]["folderTime"].ToString();
                            PointPairList list1 = new PointPairList();
                            for (int j = 0; j < dtValue.Rows.Count; j++)
                            {
                                double x;
                                float y;
                                if (num == 1)
                                {
                                    x = float.Parse(dtValue.Rows[j]["TM"].ToString());
                                    y = float.Parse(dtValue.Rows[j]["Depth"].ToString()) - wellzero;
                                }
                                else
                                {
                                    x = float.Parse(dtValue.Rows[j]["Depth"].ToString()) - wellzero;
                                    y = float.Parse(dtValue.Rows[j]["TM"].ToString());
                                }
                                list1.Add(x, y);
                            }
                            if (list1.Count == 0)//如果曲线没有数据
                            {
                                messageError += "时间" + (MainForm.getInstance().TimeList.Items[i]).ToString() + "无数据\n";
                                continue;
                            }
                            else
                            {
                                Color co = ZedGraphClass.GetColor(i);
                                LineItem _lineitem2 = gp.AddCurve(hn[i], list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
                                _lineitem2.Line.Width = 2.0F;//线的宽度
                                //string la = _lineitem2.Label.Text.ToString();
                                if (drawAttribute.Linenum == 2)
                                {
                                    _lineitem2.Line.IsVisible = false;
                                }
                                _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                                gp.AxisChange();
                                MainForm.getInstance().GTime.Refresh();
                            }

                        }

                    }

                }
                else if (SQLTableTime.Rows.Count >15)
                {
                    MessageBox.Show("时间区间太大，曲线条数大于15");
                }
                else
                {
                    if (messageError == null)
                    {
                        MessageBox.Show("没有添加时间点！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                       
                }
                MainForm.getInstance().groupBox4.Enabled = true;
                MainForm.getInstance().GTime.Enabled = true;
                mycon.Close();
                if (messageError != null)
                {
                    MessageBox.Show("以下时间点无数据！\n" + messageError, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

      //public  static ArrayList getTime()//得到曲线的时间集合
      //  {
      //      ArrayList ATime = new ArrayList();
      //      MySqlConnection mycon = new MySqlConnection();
      //      mycon = getMycon();

      //      try
      //      {
      //          //string sss = MainForm.getInstance().TimeList.Text;
      //          //String[] strs = sss.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      //          for (int i = 0; i < MainForm.getInstance().GTimeList.Items.Count; i++)//将单项深度导入int数组，看是否正确
      //          {

      //              string str = "SELECT DISTINCT folderTime from allgrat_data WHERE folderTime >=  \'" + Convert.ToDateTime(MainForm.getInstance().GTimeList.Items[i]) + "\' order by folderTime";
      //              System.Data.DataTable dt = getDataTable(str, mycon);
      //              ATime.Add(dt.Rows[0]["folderTime"].ToString());
      //          }
      //      }
      //      catch //深度格式不正确
      //      {
      //          MessageBox.Show("单项时间输入格式不正确\n或输入的部分时间点不存在数据", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      //          bu = 1;
      //      }
      //      mycon.Close();
      //      return ATime;
      //  }
      //public static ArrayList getfolderTable()//得到曲线duozai表名称
      //  {
      //      ArrayList ATime = new ArrayList();
      //      MySqlConnection mycon = new MySqlConnection();
      //      mycon = getMycon();
      //          try
      //          {
      //              //string sss = MainForm.getInstance().TimeList.Text;
      //              //String[] strs = sss.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      //              for (int i = 0; i < MainForm.getInstance().GTimeList.Items.Count; i++)//将单项深度导入int数组，看是否正确
      //              {

      //                  string str = "SELECT DISTINCT folderTable from allgrat_data WHERE folderTime >=  \'" + Convert.ToDateTime(MainForm.getInstance().GTimeList.Items[i]) + "\' order by folderTime";
      //                  System.Data.DataTable dt = getDataTable(str, mycon);
      //                  ATime.Add(dt.Rows[0]["folderTable"].ToString());
      //              }
      //          }
      //          catch //深度格式不正确
      //          {
      //              MessageBox.Show("单项时间输入格式不正确\n或输入时间不存在数据", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      //              bu = 1;
      //          }
            
      //      mycon.Close();
      //      return ATime;
      //  }
      public static DataTable getGTime(MySqlConnection mycon)//得到曲线的时间集合
      {
          DataTable dtTimeTable = new DataTable();
          dtTimeTable.Columns.Add("folderTime");
          dtTimeTable.Columns.Add("folderTable");
          messageError = null;
          for (int i = 0; i < MainForm.getInstance().GTimeList.Items.Count; i++)//将单项深度导入int数组，看是否正确
          {
              string str = "SELECT folderTime,folderTable  from allgrat_data WHERE folderTime >=  \'" + Convert.ToDateTime(MainForm.getInstance().GTimeList.Items[i]) + "\' order by folderTime  limit 1 ";
              System.Data.DataTable dt = getDataTable(str, mycon);
              if (dt.Rows.Count != 0)
              {
                  DataRow dr = dtTimeTable.NewRow();
                  dr["folderTime"] = dt.Rows[0]["folderTime"];
                  dr["folderTable"] = dt.Rows[0]["folderTable"];
                  dtTimeTable.Rows.Add(dr);
              }
              else
              {
                  messageError += (MainForm.getInstance().GTimeList.Items[i]).ToString() + '\n';
              }
          }
          DataTable distinckTable = MyDataTable.getDistinckTable(dtTimeTable);
          return distinckTable;
      }      
       
        //public static ArrayList getAlist(ArrayList h)
        //{
        //    ArrayList h2 = new ArrayList();
        //    String[] Dep = new String[h.Count];
        //    for (int i = 0; i < h.Count; i++)
        //    {
        //        string a = drawingzgcDep.getName(Convert.ToDateTime(h[i]), "dts", "上旬");
        //        string b = drawingzgcDep.getName(Convert.ToDateTime(h[i]), "dts", "中旬");
        //        string c = drawingzgcDep.getName(Convert.ToDateTime(h[i]), "dts", "下旬");
        //        //ArrayList ADepth = new ArrayList();
        //        //if (h2.Contains(a))
        //        //{
        //        //    h2.Add(a);
        //        //}
        //        //if (h2.Contains(b))
        //        //{
        //        //    h2.Add(b);
        //        //}
        //        //if (h2.Contains(c))
        //        //{
        //        //    h2.Add(c);
        //        //}


        //        int id = Array.IndexOf(Dep, a);
        //        if (id == -1)  //不存在
        //        {
        //            // h2[i*3] = a;
        //            h2.Add(a);
        //        }
        //        id = Array.IndexOf(Dep, b);
        //        if (id == -1)  //不存在
        //        {
        //            // h2[i * 3] = a;
        //            h2.Add(c);
        //        }
        //        id = Array.IndexOf(Dep, c);
        //        if (id == -1)  //不存在
        //        {
        //            //  h2[i*3] =c;
        //            h2.Add(c);
        //        }




        //    }
        //    return h2;
        //}
    }
}
