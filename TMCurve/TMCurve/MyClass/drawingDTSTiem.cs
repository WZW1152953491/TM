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

    class drawingzgcTime:MyClass
    {


        public static float DintervalDepth3;//区间深度1
        public static float DintervalDepth4;//区间深度2
        public static string strTime;
        public static string messageError=null;
        public static int num=0;//横纵坐标变化


        private static void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)//鼠标移动出现虚线
        {
            using (Graphics gc = MainForm.getInstance().zgcTime.CreateGraphics())
            using (Pen pen = new Pen(Color.Gray))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                RectangleF rect = MainForm.getInstance().zgcTime.GraphPane.Chart.Rect;
                //确保在画图区域
                if (rect.Contains(e.Location))
                {
                    MainForm.getInstance().zgcTime.Refresh();
                    gc.DrawLine(pen, e.X, rect.Top, e.X, rect.Bottom);
                    gc.DrawLine(pen, rect.Left, e.Y, rect.Right, e.Y);
                }
            }

        }

       
        public static void DrawingTime()//画深度温度曲线
        {      //获取井口位置
            float wellzero = ZedGraphClass.getWellZero();
            string SQLstr = getstrT(wellzero);//获取深度的限制条件
            if(SQLstr!=null)
            {
                MainForm.getInstance().groupBox3.Enabled = false;
                MainForm.getInstance().zgcTime.Enabled = false;
                //连接数据库
                MySqlConnection mycon = new MySqlConnection();
                mycon = getMycon();
                //是否循环
                string button = MainForm.getInstance().number.Text;
                //横纵坐标变化
                num = int.Parse(MainForm.getInstance().exchange.Text);

                DataTable SQLTableTime = getTime(mycon);//获取时间和表名称对应的值
                if (SQLTableTime.Rows.Count <= 15 && SQLTableTime.Rows.Count > 0)//15条线之内
                {
                    ZedGraph.GraphPane gp = MainForm.getInstance().zgcTime.GraphPane;
                    gp.GraphObjList.Clear();
                    gp.CurveList.Clear();//清除上一步画的图
                    if (MainForm.getInstance().label1.Text == "1")
                    {
                        MainForm.getInstance().zgcTime.IsEnableVZoom = true;//Y轴缩放
                        MainForm.getInstance().zgcTime.IsEnableHZoom = true;//x轴缩放

                    }
                    else
                    {

                        MainForm.getInstance().zgcTime.IsEnableVZoom = false;//禁止Y轴缩放
                        MainForm.getInstance().zgcTime.IsEnableHZoom = true;//x轴缩放


                    }
                    MainForm.getInstance().zgcTime.PointValueEvent += new ZedGraphControl.PointValueHandler(ZedGraphClass.MyPointValueHandlerTime);//设置节点信息显示样式
                    //MainForm.getInstance().zgcTime.IsShowHScrollBar = true;//横向滚动条
                    MainForm.getInstance().zgcTime.MouseMove += zedGraphControl1_MouseMove;//鼠标在图上移动出现x虚线
                    MainForm.getInstance().zgcTime.IsShowPointValues = true;//
                    MainForm.getInstance().zgcTime.IsZoomOnMouseCenter = false;   //使用滚轮时以鼠标所在点进行缩放还是以图形中心进行缩放。

                    //设置XY轴的起始点
                    if (num == 1)
                    {
                        gp.YAxis.Scale.Min = float.Parse(MainForm.getInstance().DintervalDepth3.Text);
                        gp.YAxis.Scale.Max = float.Parse(MainForm.getInstance().DintervalDepth4.Text);
                    }
                    else
                    {
                        if (MainForm.getInstance().DintervalTM3.Text != "" && MainForm.getInstance().DintervalTM4.Text != "")
                        {
                            gp.YAxis.Scale.Min = float.Parse(MainForm.getInstance().DintervalTM3.Text);
                            gp.YAxis.Scale.Max = float.Parse(MainForm.getInstance().DintervalTM4.Text);

                        }
                        else
                        {
                            gp.YAxis.Scale.MaxAuto = true;
                            gp.YAxis.Scale.MinAuto = true;
                        }
                    }


                    try
                    {
                        if (num == 1)
                        {
                            gp.XAxis.Scale.Min = float.Parse(MainForm.getInstance().DintervalTM3.Text);
                            gp.XAxis.Scale.Max = float.Parse(MainForm.getInstance().DintervalTM4.Text);
                        }
                        else
                        {
                            gp.XAxis.Scale.Min =float.Parse(MainForm.getInstance().DintervalDepth3.Text);
                            gp.XAxis.Scale.Max = float.Parse(MainForm.getInstance().DintervalDepth4.Text);
                        }

                    }
                    catch
                    {
                        //MessageBox.Show("温度区间没有选着");
                    }


                    MainForm.getInstance().zgcTime.IsAutoScrollRange = false;
                    MainForm.getInstance().zgcTime.ScrollMaxX = DintervalDepth4 + 2;//最大
                    MainForm.getInstance().zgcTime.ScrollMinX = DintervalDepth3 - 2;//最小
                    //ZedGraphClass.stile(gp);//在程序打开时已经加载过了
                    //坐标轴标题格式
                    // gp.Title.Text = "LD27-2平台 A22H井 温度深度曲线"; //图表轴名称
                    if (num == 1)
                    {
                        //gp.XAxis.Title.Text = MainForm.getInstance().YDTS2.Text; //X轴名称;
                        //gp.YAxis.Title.Text = MainForm.getInstance().YDTS2.Text;//Y轴名称
                        gp.XAxis.Title.Text = "温度（℃）"; //X轴名称;
                        gp.YAxis.Title.Text = "深度（m）"; //Y轴名称
                        gp.YAxis.Scale.IsReverse = true;//Y轴值翻转，图像一样翻转
                    }
                    else
                    {
                        gp.XAxis.Title.Text = "深度（m）"; //X轴名称;
                        gp.YAxis.Title.Text = "温度（℃）"; //Y轴名称
                        gp.YAxis.Scale.IsReverse = false;//Y轴值翻转，图像一样翻转
                    }


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
                                Thread.Sleep(1500);
                                string Str = null;
                                DataTable dtValue = new DataTable();
                                Str = "SELECT Depth,RecordTime,TM from " + SQLTableTime.Rows[i]["folderTable"] + " " + SQLstr + " and  RecordTime=\'" + Convert.ToDateTime(SQLTableTime.Rows[i]["folderTime"]) + "\' ORDER BY Depth";
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
                                if (list1.Count == 0 && k ==0)//如果曲线没有数据
                                {
                                    //已经过滤掉了无用的点
                                    messageError += "时间" + (MainForm.getInstance().TimeList.Items[i]).ToString() + "无数据\n";
                                    continue;
                                }
                                else
                                {
                                    Color co = ZedGraphClass.GetColor(i);
                                    LineItem _lineitem2 = gp.AddCurve(hn[i], list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
                                    if(drawAttribute.Linenum==2){
                                        _lineitem2.Line.IsVisible = false;
                                    }
                                
                                    _lineitem2.Line.Width = 2.0F;//线的宽度
                                    _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                    _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色                          
                                    gp.AxisChange();
                                    MainForm.getInstance().zgcTime.Refresh();
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
                            Str = "SELECT Depth,RecordTime,TM from " + SQLTableTime.Rows[i]["folderTable"] + " " + SQLstr + " and  RecordTime=\'" + Convert.ToDateTime(SQLTableTime.Rows[i]["folderTime"]) + "\' ORDER BY Depth";
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

                                messageError += "时间"+(MainForm.getInstance().TimeList.Items[i]).ToString() + "无数据\n";
                                continue;
                            }
                            else
                            {
                                Color co = ZedGraphClass.GetColor(i);
                                LineItem _lineitem2 = gp.AddCurve(hn[i], list1, ZedGraphClass.GetColor(i), SymbolType.Circle);
                                if (drawAttribute.Linenum == 2)
                                {
                                    _lineitem2.Line.IsVisible = false;
                                }
                                _lineitem2.Line.Width = 2.0F;//线的宽度
                                string la = _lineitem2.Label.Text.ToString();
                                _lineitem2.Symbol.Size = 2.4F;//线上节点的大小
                                _lineitem2.Symbol.Fill = new Fill(co);//线上节点的颜色
                                gp.AxisChange();
                                MainForm.getInstance().zgcTime.Refresh();
                            }

                        }

                    }
                 
                }
                else if (SQLTableTime.Rows.Count > 15)
                {

                        MessageBox.Show("时间点过多，曲线条数大于15！！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);               
                }
                else
                {
                    if (messageError == null)
                    {
                        MessageBox.Show("没有添加时间点！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    }
                       
                }

                MainForm.getInstance().groupBox3.Enabled = true;
                MainForm.getInstance().zgcTime.Enabled = true;
                mycon.Close();
            }
            else
            {
                MessageBox.Show("深度区间填写不正确，请重新填写！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
            if (messageError != null)
            {
                MessageBox.Show("以下时间点无数据！\n" + messageError, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        public static string getstrT(float wellzero)//获取部分SQL语句
        {
           // int wellzero =ZedGraphClass.getWellZero();
            if (MainForm.getInstance().DintervalDepth3.Text != "" && MainForm.getInstance().DintervalDepth4.Text != "")
            {
                DintervalDepth3 = float.Parse(MainForm.getInstance().DintervalDepth3.Text) + wellzero;
                DintervalDepth4 = float.Parse(MainForm.getInstance().DintervalDepth4.Text) + wellzero;
                if (DintervalDepth4 > DintervalDepth3 && DintervalDepth3 > 0)
                {
                    strTime = "WHERE Depth BETWEEN  " + DintervalDepth3 + " and " + DintervalDepth4;
                }
                else
                {
                    strTime = null;

                }
            }
            else
            {
                strTime = null;
            }
            return strTime;
        }
        public static DataTable getTime(MySqlConnection mycon)//得到曲线的时间集合
        {
          DataTable dtTimeTable = new DataTable();
          dtTimeTable.Columns.Add("folderTime");
          dtTimeTable.Columns.Add("folderTable");
          messageError = null;
          for (int i = 0; i < MainForm.getInstance().TimeList.Items.Count; i++)//将单项深度导入int数组，看是否正确
          {
              string str = "SELECT folderTime,folderTable  from alltemporpary_data WHERE folderTime >=  \'" + Convert.ToDateTime(MainForm.getInstance().TimeList.Items[i]) + "\' order by folderTime  limit 1 ";
              System.Data.DataTable dt = getDataTable(str, mycon);
              if (dt.Rows.Count != 0)
              {
                  DataRow dr = dtTimeTable.NewRow();
                  dr["folderTime"] = dt.Rows[0]["folderTime"];
                  dr["folderTable"] = dt.Rows[0]["folderTable"];
                  dtTimeTable.Rows.Add(dr);
              }
              else {
                  messageError += (MainForm.getInstance().TimeList.Items[i]).ToString()+'\n';
              }
          }
          DataTable distinckTable = MyDataTable.getDistinckTable(dtTimeTable);
          return distinckTable;
        }      
    }
}