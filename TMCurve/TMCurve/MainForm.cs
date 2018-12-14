using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

using System.Diagnostics;
using ZedGraph;


namespace TMCurve
{

    public partial class MainForm : Form
    {
       
        MyClass.AutoSizeFormClass asc = new MyClass.AutoSizeFormClass();
        public static MySqlConnection mycon;
        public static Thread ThreadDT;//Time曲线所使用的线程
        public static Thread ThreadDD;//Depth曲线所使用的线程。
        public static Thread ThreadGD;//GDep曲线所使用的线程
        public static Thread ThreadGT;//GTime曲线所使用的线程
        public static Thread ThreadWell;//井筒图形
        public static Thread threadImport;//导入数据所用的线程
        public static Thread threadDTSreal;//DTS实时数据展示
        public static Thread threadGratreal;//光栅实时数据展示
        public static Thread threadData;
        public static int num = 1;
        private static MainForm _instance;
         // private static MainForm _instance;
        public MainForm()
        {
            _instance = this;
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//线程就能安全的访问窗体控件了//该方法是通过采用取消线程安全保护模式的方式实现的，所以不建议采用。

        }

        private static readonly object obj = new object();
        public static MainForm getInstance()
        {
              if (null == MainForm._instance)
              {
                  lock (obj)
                  {
                      if (null == MainForm._instance)
                      {
                          MainForm._instance = new MainForm();
                      }
                  }
 
              }
            return MainForm._instance;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           //string aa=shuxing.aa;//获取另一个窗体的变量值
            //string a = shuxing.getInstance().sxXtitle.Text;//获取另一个窗体的控件值
         //  shuxing.getInstance().sxXtitle.Text = "2222";
            //MainForm.getInstance().wite.Visible = false;
           GratLineNumber.SelectedIndex = 9;
           // DTSLineNumber.SelectedIndex = 9;
           //asc.controllInitializeSize(this);
           try
            {
                MyClass.CreateTable.CreateDatabase();
               
            }
            catch 
            {
               
            }
            try
            {
               
                MyClass.CreateTable.CAllTemporparyTable("alltemporpary_data");
              
            }
            catch 
            {
              
            }
            try
            {

                MyClass.CreateTable.CAllTemporparyTable("allGrat_data");//建立光栅温度的数据总表
            }
            catch
            {

            }
          
            //this.WindowState = FormWindowState.Maximized;//窗口最大化
            //MyClass.CreateTable.Depcom();//为控件添加数据。光栅温度的全部深度
            groupBoxPre.Enabled = false;//压力相关的为不可用
            groupBoxPreExport.Enabled = false;//压力相关的为不可用
            //errorDTS.Visible = false;
            //errorFBR.Visible = false;
          
            //MyClass.MyClass.PatleWell();
            MyClass.drawAttribute.morenAttribute();

           //DTSDepth曲线属性设置
            zgcDep.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//设置汉化属性
            ZedGraph.GraphPane gp =zgcDep.GraphPane;
            gp.Title.Text = "LD27-2平台 A22H井 DTS时间温度曲线"; //图表轴名称                          
            gp.XAxis.Title.Text = "时间"; //图表轴名称       
            gp.YAxis.Title.Text = "温度(℃)"; //图表轴名称
            MyClass.ZedGraphClass.stile(gp);//在程序打开时已经加载过了
            zgcDep.IsEnableVZoom = false;//禁止Y轴缩放
            zgcDep.IsEnableHZoom = false;//禁止x轴缩放
            zgcDep.IsEnableHPan = false; //禁止横向移动;
            zgcDep.IsEnableVPan = false; //禁止纵向移动;
            gp.YAxis.Scale.MaxAuto = true;
            gp.XAxis.Scale.FontSpec.Size = 9f;//X轴刻度
            gp.YAxis.Scale.FontSpec.Size = 9f;//Y轴刻度


            //DTSTime曲线属性设置
            ZedGraph.GraphPane gp1 = zgcTime.GraphPane;
            zgcTime.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//汉化属性  
            gp1.Title.Text = "LD27-2平台 A22H井 DTS深度温度曲线"; //图表轴名称
            gp1.XAxis.Title.Text = "深度(m)"; //X轴名称
            gp1.YAxis.Title.Text = "温度(℃)"; //Y轴名称
            MyClass.ZedGraphClass.stile(gp1);
            zgcTime.IsEnableVZoom = false;//禁止Y轴缩放
            zgcTime.IsEnableHZoom = false;//禁止X轴缩放
            zgcTime.IsEnableHPan = false; //禁止横向移动;
            zgcTime.IsEnableVPan = false; //禁止纵向移动;
            gp1.YAxis.Scale.MaxAuto = true;
            gp1.XAxis.Scale.FontSpec.Size = 9f;//X轴刻度
            gp1.YAxis.Scale.FontSpec.Size = 9f;//Y轴刻度
            //GTime曲线属性设置
            ZedGraph.GraphPane gp3 = GTime.GraphPane;
            GTime.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//汉化属性  
            gp3.Title.Text = "LD27-2平台 A22H井 光栅深度温度曲线"; //图表轴名称
            gp3.XAxis.Title.Text = "深度(m)"; //X轴名称
            gp3.YAxis.Title.Text = "温度(℃)"; //Y轴名称
            MyClass.ZedGraphClass.stile(gp3);
            GTime.IsEnableVZoom = false;//禁止Y轴缩放
            GTime.IsEnableHZoom = false;//禁止X轴缩放
            GTime.IsEnableHPan = false; //禁止横向移动;
            GTime.IsEnableVPan = false; //禁止纵向移动;
            gp3.YAxis.Scale.MaxAuto = true;
            gp3.XAxis.Scale.FontSpec.Size = 9f;//X轴刻度
            gp3.YAxis.Scale.FontSpec.Size = 9f;//Y轴刻度


            //Grating(光栅)Depth曲线属性设置
            GDep.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//设置汉化属性
            ZedGraph.GraphPane gp2 = GDep.GraphPane;
            gp2.Title.Text = "LD27-2平台 A22H井 光栅时间温度曲线"; //图表轴名称                          
            gp2.XAxis.Title.Text = "时间"; //图表轴名称       
            gp2.YAxis.Title.Text = "温度(℃)"; //图表轴名称
            MyClass.ZedGraphClass.stile(gp2);//在程序打开时已经加载过了
            GDep.IsEnableVZoom = false;//禁止Y轴缩放
            GDep.IsEnableHZoom = false;//禁止x轴缩放
            GDep.IsEnableHPan = false; //禁止横向移动;
            GDep.IsEnableVPan = false; //禁止纵向移动;
            gp2.YAxis.Scale.MaxAuto = true;
            gp2.XAxis.Scale.FontSpec.Size = 9f;//X轴刻度
            gp2.YAxis.Scale.FontSpec.Size = 9f;//Y轴刻度

            //DTSReal曲线属性设置
            ZedGraph.GraphPane gp4 = DTSReal.GraphPane;
            DTSReal.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//汉化属性  
            gp4.Title.Text = "LD27-2平台 A22H井 DTS深度温度实时曲线"; //图表轴名称
            gp4.XAxis.Title.Text = "深度(m)"; //X轴名称
            gp4.YAxis.Title.Text = "温度(℃)"; //Y轴名称
            MyClass.ZedGraphClass.stile(gp4);
            DTSReal.IsEnableVZoom = false;//禁止Y轴缩放
            DTSReal.IsEnableHZoom = false;//禁止X轴缩放
            DTSReal.IsEnableHPan = false; //禁止横向移动;
            DTSReal.IsEnableVPan = false; //禁止纵向移动;
       
            gp4.YAxis.Scale.MaxAuto = true;
            gp4.XAxis.Scale.FontSpec.Size = 9f;//X轴刻度
            gp4.YAxis.Scale.FontSpec.Size = 9f;//Y轴刻度
            gp4.XAxis.Scale.Min = 0;//X轴的最小值为0

            //GratReal曲线属性设置
            ZedGraph.GraphPane gp5 = GratReal.GraphPane;

           GratReal.IsEnableHPan = false; //禁止横向移动;
           GratReal.IsEnableVPan = false; //禁止纵向移动;
           GratReal.IsEnableVZoom = false;//禁止Y轴缩放
           GratReal.IsEnableHZoom = false;//禁止X轴缩放
            GratReal.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//汉化属性  
            gp5.Title.Text = "LD27-2平台 A22H井 光栅深度温度实时曲线"; //图表轴名称
            gp5.XAxis.Title.Text = "深度(m)"; //X轴名称
            gp5.YAxis.Title.Text = "温度(℃)"; //Y轴名称
            MyClass.ZedGraphClass.stile(gp5);
           
            gp5.YAxis.Scale.MaxAuto = true;
            gp5.XAxis.Scale.FontSpec.Size = 9f;//X轴刻度
            gp5.YAxis.Scale.FontSpec.Size = 9f;//Y轴刻度
           // gp5.XAxis.Scale.Min = 0;//X轴的最小值为0

            //管柱图曲线属性设置
            ZedGraph.GraphPane myPane = MainForm.getInstance().WellTM.GraphPane;
    
            WellTM.IsEnableVZoom = false;//Y轴缩放
            WellTM.IsEnableHZoom = false;//x轴缩放
            WellTM.IsEnableHPan = false; //禁止横向移动;
            WellTM.IsEnableVPan = false; //禁止纵向移动;
            //MainForm.getInstance().WellTM.IsEnableVZoom = false;//Y轴缩放
            //MainForm.getInstance().WellTM.IsEnableHZoom = false;//x轴缩放
           // MainForm.getInstance().WellTM.IsShowPointValues = true; //显示节点坐标值
            WellTM.ContextMenuBuilder += MyClass.ZedGraphClass.MyContextMenuBuilder;//汉化属性  
            myPane.CurveList.Clear();//清除上一步画的图
            myPane.Title.Text = "LD27-2平台 A22H井 管柱图";
            myPane.YAxis.Title.IsVisible = false;
            myPane.XAxis.Title.IsVisible = false;
          // myPane.X2Axis.Title.Text = "水平长度（m）";
            //myPane.YAxis.Title.Text = "深度（m）"; //Y轴名称
            ////myPane.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);//控件颜色填充
            ////myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);//画板颜色填充
               MainForm.getInstance().WellTM.IsEnableVZoom = false;//Y轴缩放
            MainForm.getInstance().WellTM.IsEnableHZoom = false;//x轴缩放
            MainForm.getInstance().WellTM.IsEnableHPan = false; //禁止横向移动;
            MainForm.getInstance().WellTM.IsEnableVPan = false; //禁止纵向移动;
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
            MyClass.ZedGraphClass.stile(myPane);//在程序打开时已经加载过了
          //  myPane.YAxis.Title.FontSpec.Angle = 90; //标题的偏转角度
            
            //myPane.X2Axis.Scale.Min = 0;
            //myPane.X2Axis.Scale.Max = 2000;
            //myPane.YAxis.Scale.Min = 0;
            //myPane.YAxis.Scale.Max = 2000;
            //myPane.AxisChange();
            Point topLeft = new Point(45, 45);//原点坐标
            Size howBig = new Size(630, 450);//画图区域大小
            Rectangle rectangleArea = new Rectangle(topLeft, howBig);
            myPane.Chart.Rect = rectangleArea;
            //myPane.XAxis.Scale.Min = 0;
            //myPane.XAxis.Scale.Max = 1800;
            //myPane.YAxis.Scale.Min = 0;
            //myPane.YAxis.Scale.Max =1800;
            //myPane.AxisChange();//为了上面四行代码正确生效
            //Size howBig = new Size(600, 400);//画图区域大小
        }

        private void button6_Click(object sender, EventArgs e)//Dep曲线的生成
        {  
            if (ThreadDD != null)
            {
                ThreadDD.Abort();
            }
            number.Text = "one";
            ThreadDD = new Thread(MyClass.drawingDTSDep.DrawingDep);
            ThreadDD.Start();
            
        }

        private void button2_Click(object sender, EventArgs e)//time曲线生成
        {
            if (ThreadDT != null)
            {
                ThreadDT.Abort();
            }
            number.Text = "one";
            ThreadDT = new Thread( MyClass.drawingzgcTime.DrawingTime);
            ThreadDT.Start();
        }

        private void button5_Click(object sender, EventArgs e)//清除时间列表
        {
            MyClass.ZedGraphClass.clear_Time();
         
            if (ThreadDT != null)
            {
                ThreadDT.Abort();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //TimeList.Text = TimeList.Text + DintervalTime5.Value + ",";
            TimeList.Items.Add(DintervalTime5.Value);
            TimeList.Text = "点击选项删除该项";
        }

        private void button1_Click(object sender, EventArgs e)//time曲线循环生成
        {
            if (ThreadDT != null) 
            { ThreadDT.Abort(); }
            number.Text = "two";
            ThreadDT = new Thread(MyClass.drawingzgcTime.DrawingTime);
            // Thread.CurrentThread.Name = "主线程";
            ThreadDT.Start();
        }
        private void zgcTime_Click(object sender, EventArgs e)//终止time曲线循环生成
        {
            if (number.Text == "two")
            {
                ThreadDT.Abort();
                number.Text = "one";
                MyClass.drawingzgcTime.DrawingTime();
            }
        }

        private void button7_Click(object sender, EventArgs e)//Dep曲线循环生成
        {
            if (ThreadDD != null) { ThreadDD.Abort(); }

            number.Text = "two";
            ThreadDD = new Thread(MyClass.drawingDTSDep.DrawingDep);
            ThreadDD.Start();
            //thread1.IsBackground = true;
        }
        private void zgcDep_Click(object sender, EventArgs e)//终止Dep曲线循环生成
        {
            
        }
       
        public void clearDep_Click(object sender, EventArgs e)
        {
         
           MyClass.ZedGraphClass.clear_Depth();
           if (ThreadDD != null)
           {
               ThreadDD.Abort();
           }
           //DateTime a = DintervalTime1.Value;
           //DateTime b= DintervalTime2.Value;
           //MyClass.drawingzgcDep.getTNameTable(a, b);
        }
        private void tabControl1_Click(object sender, EventArgs e)
        {
            //MyClass.ZedGraphClass.clear_Time();
            //MyClass.ZedGraphClass.clear_Depth();
            string aa = tabControl1.SelectedTab.Name.ToString();
            if (aa == "GratDep")
            {
                //MyClass.CreateTable.Depcom();
            }
            
        }


        private void export_Click(object sender, EventArgs e)
        {
            groupBoxDTS.Enabled = false;
            groupBoxGrating.Enabled = false;
            groupBoxPre.Enabled = false;
            groupBoxWell.Enabled = false;
            groupBoxTMExport.Enabled = false;
            groupBoxPreExport.Enabled = false;
            exportLable.Enabled = true;
           // MyClass.export.exportData();
            if (threadImport == null)
            {
                threadImport = new Thread(MyClass.export.exportData);
                threadImport.SetApartmentState(ApartmentState.STA); // 设置为单线程单元(STA)状态 可以弹出选着文件夹
                threadImport.Start();
            
            }
            else
            {
                 if (MainForm.threadImport.IsAlive == true)//当线程为不为时，判断是否在运行，在运行则提示线程依旧在工作
                {
                    MessageBox.Show("已有导出任务，请稍后！");
                }
                else
                {
                    threadImport = new Thread(MyClass.export.exportData);
                    threadImport.SetApartmentState(ApartmentState.STA); // 设置为单线程单元(STA)状态 可以弹出选着文件夹
                    threadImport.Start();  
                }

            }
         
            //Thread t = new Thread(MyClass.export.exportData);
            //t.SetApartmentState(ApartmentState.STA); // 设置为单线程单元(STA)状态
            //t.Start();
            
      
        }

        private void reset_Click(object sender, EventArgs e)
        {
            intervalTime1.Value = System.DateTime.Now;
            intervalTime2.Value = System.DateTime.Now;
            intervalDepth1.Text = "";
            intervalDepth2.Text = "";
            intervalTM1.Text = "";
            intervalTM2.Text = "";
            singleDepth.Text = "";
        }

   

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIndex = TimeList.SelectedIndex;
            //移除所选项
            TimeList.Items.RemoveAt(selectIndex);
        }


        private void button5_Click_1(object sender, EventArgs e)
        {
            groupBoxDTS.Enabled = false;
            groupBoxGrating.Enabled = false;
            groupBoxPre.Enabled = false;
            groupBoxWell.Enabled = false;
            groupBoxTMExport.Enabled = false;
            groupBoxPreExport.Enabled = false;
            if (threadImport != null)
            {
                threadImport.Abort();
            }
            threadImport = new Thread(MyClass.importWellTM.importWell);
            threadImport.Start();

            //MainForm.getInstance().button11.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
           // MyClass.WellTM.DrawingWell();
            groupBox8.Enabled = false;
            if (ThreadWell != null)
            {
                ThreadWell.Abort();
            }
            ThreadWell = new Thread(MyClass.WellTM.DrawingWell);
            ThreadWell.Start();
        }



        private void button6_Click_1(object sender, EventArgs e)
        {
            MyClass.drawAttribute.baocunAttribute();
          
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            MyClass.drawAttribute.morenAttribute();
            MyClass.drawAttribute.baocunAttribute();
        }
       

        private void button15_Click(object sender, EventArgs e)
        {
            groupBoxDTS.Enabled = false;
            groupBoxGrating.Enabled = false;
            groupBoxPre.Enabled = false;
            groupBoxWell.Enabled = false;
            groupBoxTMExport.Enabled = false;
            groupBoxPreExport.Enabled = false;
            FBRimportLable.Visible = true;
            if (threadImport != null)
            {
                threadImport.Abort();
            }
            threadImport = new Thread(MyClass.importGrat.ImportGrat);
            threadImport.Start();
        }

        private void button21_Click(object sender, EventArgs e)
        {

        }

        private void selectDTSFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";
            //如果路径不为空
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                groupBoxDTS.Enabled = false;
                groupBoxGrating.Enabled = false;
                groupBoxPre.Enabled = false;
                groupBoxWell.Enabled = false;
                groupBoxTMExport.Enabled = false;
                groupBoxPreExport.Enabled = false;
                DTSimportLable.Visible = true;
                dataFolder.Text = null;
                dataFolder.Text = dialog.SelectedPath;
                if (threadImport != null)
                {
                    threadImport.Abort();
                }
                threadImport = new Thread(MyClass.importDTS.selectFolder);
                threadImport.Start();
            }
          
            //当选择文件夹时，先进行temporpary_data的重新创建，并且其他用到此表的功能没有在使用或全部禁用
            
        }
        //多线程、分表导入数据库的方法
        private void btDTSimport_Click(object sender, EventArgs e)
        {
            groupBoxDTS.Enabled = false;
            groupBoxGrating.Enabled = false;
            groupBoxPre.Enabled = false;
            groupBoxWell.Enabled = false;
            groupBoxTMExport.Enabled = false;
            groupBoxPreExport.Enabled = false;
            DTSimportLable.Visible = true;
            if (threadImport != null)
            {
                threadImport.Abort();
            }
            threadImport = new Thread(MyClass.importDTS.ImportDts);
            threadImport.Start();
           
          
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

            if (realDTS1.Text != realDTS2.Text && realGrat1.Text != realGrat2.Text)
            {
                try
                {
                    //如果都是整数，则继续监控（已经在控件的KeyPress控件中设置完成了整数的输入验证）
                    //int a=Convert.ToInt32(realGrat4.Text);
                    //int b=Convert.ToInt32(realDTS4.Text);
                    groupBox28.Enabled = false;
                    TimeLook.Enabled = false;
                    if (realDTS1.Text != "" && realDTS2.Text != "" && realGrat1.Text != "" && realGrat2.Text != "")//
                    {
                        threadDTSreal = new Thread(MyClass.importDTSreal.jiankong);
                        threadDTSreal.SetApartmentState(ApartmentState.STA); //
                        threadDTSreal.Start();
                        threadGratreal = new Thread(MyClass.importGratreal.jiankongGrat);
                        threadGratreal.SetApartmentState(ApartmentState.STA); //
                        threadGratreal.Start();
                        TimeLook.Text = "正在监控";
                        TimeLook.BackColor = Color.Green;
                    }
                    else
                    {
                        MessageBox.Show("DTS/光栅的部分路径条件填写不完整");
                        groupBox28.Enabled = true;
                        TimeLook.Enabled = true;
                    }

                }
                catch
                {
                    MessageBox.Show("数据填写不正确");
                }
            }
            else {
                MessageBox.Show("同一数据的源文件夹路径与目标文件夹路径不能相同");
            }
            
            

           
        }

        private void selectGratingFolder_Click(object sender, EventArgs e)
        {
            //当选择文件夹时，先进行特，temporpary_data的重新创建，并且其他用到此表的功能没有在使用或全部禁用
          
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                groupBoxDTS.Enabled = false;
                groupBoxGrating.Enabled = false;
                groupBoxPre.Enabled = false;
                groupBoxWell.Enabled = false;
                groupBoxTMExport.Enabled = false;
                groupBoxPreExport.Enabled = false;
                FBRimportLable.Visible = true;
                GratFolder.Text = null;
                GratFolder.Text = dialog.SelectedPath;
                if (threadImport != null)
                {
                    threadImport.Abort();
                }
                threadImport = new Thread(MyClass.importGrat.selectFolderGrat);
                threadImport.Start();  

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox21.Text = dialog.FileName;     
            }
            else
            {
                MessageBox.Show("没有文件");
            }
        }

        private void groupBox23_Enter(object sender, EventArgs e)
        {

        }

        private void groupBoxTMExport_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void selectPreFolder_Click(object sender, EventArgs e)
        {

        }

        private void groupBoxGrating_Enter(object sender, EventArgs e)
        {

        }

        private void GratDep_Click(object sender, EventArgs e)
        {

        }

        private void Depcom_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIndex = Depcom.SelectedIndex;
           
            //移除所选项
            string a =Depcom.Items[selectIndex].ToString();
            textBox3.Text = textBox3.Text + a + ",";
            Depcom.Items.RemoveAt(selectIndex);
           
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox3.Text =null;
            for (int i = 0; i < Depcom.Items.Count; i++)
            {
                textBox3.Text = textBox3.Text + Depcom.Items[i] + ",";
            } 
        
            
        }

        private void label77_Click(object sender, EventArgs e)
        {

        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            if (ThreadGD != null)
            {
                ThreadGD.Abort();
            }
            number.Text = "one";
            ThreadGD = new Thread(MyClass.drawingGDep.DrawingGratDep);
            ThreadGD.Start();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (ThreadGD != null) { ThreadGD.Abort(); }
            number.Text = "two";
            ThreadGD = new Thread(MyClass.drawingGDep.DrawingGratDep);
            ThreadGD.Start();
        }

        private void zgcDep_Load(object sender, EventArgs e)
        {

        }

        private void GDep_Click(object sender, EventArgs e)
        {
            if (number.Text == "two")
            {
                if (ThreadGD != null)
                {
                    ThreadGD.Abort();
                }
                number.Text = "one";
               // MyClass.drawingGDep.DrawingGratDep();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            GTimeList.Items.Add(GTime5.Value);
            GTimeList.Text = "点击选项删除该项";
        }

        private void button25_Click(object sender, EventArgs e)
        {
            MyClass.ZedGraphClass.clear_GratTime();
            if (ThreadGT != null)
            {
                ThreadGT.Abort();
            }
        }

        private void GratDepClear_Click(object sender, EventArgs e)
        {
            MyClass.ZedGraphClass.clear_GratDepth();
            if (ThreadGD != null)
            {
                ThreadGD.Abort();
            }
        }

        private void GTimeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIndex = GTimeList.SelectedIndex;
            //移除所选项
            GTimeList.Items.RemoveAt(selectIndex);
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            if (ThreadGT != null)
            {
                ThreadGT.Abort();
            }
            number.Text = "one";
            ThreadGT = new Thread(MyClass.drawingGTime.DrawingGratTime);
            // Thread.CurrentThread.Name = "主线程";
            ThreadGT.Start();
        }

        private void button10_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选取原文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                realGrat1.Text = null;
                realGrat1.Text = dialog.SelectedPath;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选取目标文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                realGrat2.Text = null;
                realGrat2.Text = dialog.SelectedPath;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选取原文件夹";


            if (dialog.ShowDialog() == DialogResult.OK)
            {
                realDTS1.Text = null;
                realDTS1.Text = dialog.SelectedPath;
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选取目标文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                realDTS2.Text = null;
                realDTS2.Text = dialog.SelectedPath;//选取的文件夹路径
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
           // threadGratreal.Abort();
            
        }

        private void button33_Click(object sender, EventArgs e)
        {
            //threadGratreal.Start();
        }

        private void label64_Click(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (threadData != null)
            {
                threadData.Abort();
            }
            DataDo.Enabled = false;
            
            threadData = new Thread(MyClass.DataBaseDo.reductionDataBase);
            threadData.SetApartmentState(ApartmentState.STA); // 设置为单线程单元(STA)状态,可以使用Openfolder等默认控件
            threadData.Start();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (threadData != null)
            {
                threadData.Abort();
            }
            DataDo.Enabled = false;
            threadData = new Thread(MyClass.DataBaseDo.backupDataBase);
            threadData.SetApartmentState(ApartmentState.STA); // 设置为单线程单元(STA)状态
            threadData.Start();

            
           // t.Start();

        }

        private void Property_Click(object sender, EventArgs e)
        {

        }

        private void realDTS4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }

        private void realGrat4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
            /*
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (textBox1.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(textBox1.Text, out oldf);
                    b2 = float.TryParse(textBox1.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }*/
        }

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }

        private void groupBox21_Enter(object sender, EventArgs e)
        {

        }

        private void DintervalDepth4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (DintervalDepth4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(DintervalDepth4.Text, out oldf);
                    b2 = float.TryParse(DintervalDepth4.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void DintervalDepth3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (DintervalDepth3.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(DintervalDepth3.Text, out oldf);
                    b2 = float.TryParse(DintervalDepth3.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //DateTime dt = System.DateTime.Now;
            //DateTime dt1 = dt;
           
        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }

        }

        private void groupBox13_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void groupBox27_Enter(object sender, EventArgs e)
        {

        }

        private void Diameter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Diameter1.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Diameter1.Text, out oldf);
                    b2 = float.TryParse(Diameter1.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Diameter2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Diameter2.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Diameter2.Text, out oldf);
                    b2 = float.TryParse(Diameter2.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Diameter3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Diameter3.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Diameter3.Text, out oldf);
                    b2 = float.TryParse(Diameter3.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Diameter4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Diameter3.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Diameter3.Text, out oldf);
                    b2 = float.TryParse(Diameter3.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Length1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Length1.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Length1.Text, out oldf);
                    b2 = float.TryParse(Length1.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Length2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Length2.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Length2.Text, out oldf);
                    b2 = float.TryParse(Length2.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Length3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Length3.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Length3.Text, out oldf);
                    b2 = float.TryParse(Length3.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void plateName_TextChanged(object sender, EventArgs e)
        {

        }

        private void plateName_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.plateName.ImeMode = ImeMode.Off;

            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z') ||
                  (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar == 8) || (e.KeyChar == '_'))
            {
                e.Handled = false;
            }

            else
            {
                MessageBox.Show("用户名只能为字母、数字和下划线！");
                e.Handled = true;
            }
        }

        private void wellName_TextChanged(object sender, EventArgs e)
        {

        }

        private void wellName_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.wellName.ImeMode = ImeMode.Off;

            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z') ||
                  (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar == 8) || (e.KeyChar == '_') || e.KeyChar == '\b')
            {
                e.Handled = false;
            }

            else
            {
                MessageBox.Show("只能为字母、数字和下划线！");
                e.Handled = true;
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (ThreadGT != null)
            {
                ThreadGT.Abort();
            }
            number.Text = "two";
            ThreadGT = new Thread(MyClass.drawingGTime.DrawingGratTime);
            // Thread.CurrentThread.Name = "主线程";
            ThreadGT.Start();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void WriteLogUrl_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.WriteLogUrl.ImeMode = ImeMode.Off;
            int len = this.WriteLogUrl.Text.Trim().Length;
            int maxLength = 1;
            if ( (e.KeyChar >= 'C' && e.KeyChar <= 'F') ||
                  (e.KeyChar >= 'c' && e.KeyChar <= 'f') || e.KeyChar == '\b' && len < maxLength)//退格键
            {
                e.Handled = false;
            }

            else 
            {

                MessageBox.Show("请填写c-f(大小写都可以)！");
            }

        }
        private Point mouse_offset;
        private void wellPicture_MouseUp(object sender, MouseEventArgs e)
        {
            //mouse_offset = new Point(-e.X, -e.Y);
        }

        private void wellPicture_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(e.X, e.Y);
            //if (e.Button == MouseButtons.Left)
            //{
            //    Point mousePos = Control.MousePosition;
            //    mousePos.Offset(mouse_offset.X, mouse_offset.Y);
            //    ((Control)sender).Location = ((Control)sender).Parent.PointToClient(mousePos);
            //}
        }

        private void WellZero1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }

        }

        private void tabControl2_SizeChanged(object sender, EventArgs e)
        {
            
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
           // asc.controlAutoSize(this);
        }

        private void wellPicture_MouseMove(object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;
            //((Control)sender).Cursor = Cursors.Arrow;
            if (e.Button == MouseButtons.Left)
            {
                //Point mousePos = Control.MousePosition;
                //mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                //((Control)sender).Location = ((Control)sender).Parent.PointToClient(mousePos);

                control.Left = control.Left + e.X - mouse_offset.X;
                control.Top = control.Top + e.Y - mouse_offset.Y;

            }
        }

        private void errorbt_Click(object sender, EventArgs e)
        {
            errorbt.BackColor = Color.Transparent;
            errorbt.Text = "数据警告";
            tabControl2.SelectedTab = tabPage4;

        }

        private void stopLook_Click(object sender, EventArgs e)
        {
            bool b = threadDTSreal.IsAlive;
            bool a = threadDTSreal.IsAlive;
            threadDTSreal.Abort();
           
            while (threadDTSreal.ThreadState != System.Threading.ThreadState.Aborted)
            {
                Thread.Sleep(1000);
               // MessageBox.Show("something is doing "); 
            }

            threadGratreal.Abort();

          
            while (threadGratreal.ThreadState != System.Threading.ThreadState.Aborted)
            {
                Thread.Sleep(1000);
              //  MessageBox.Show("some thsing is doing "); 
            }   
             
             groupBox28.Enabled = true;
             TimeLook.Enabled = true;
             TimeLook.BackColor = Color.Transparent;
           
           
        }

        private void MainForm_Click(object sender, EventArgs e)
        {

        }

        private void realDTS3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }

        private void realGrat3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }

        private void wellzerowell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }

        private void yujignTMC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            shangfan.BackColor = Color.Transparent;
            //shangfan.Text = "数据警告";
            tabControl2.SelectedTab = tabPage4;
        }

        private void label68_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dingfeng_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (dingfeng.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(dingfeng.Text, out oldf);
                    b2 = float.TryParse(dingfeng.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void wite_Click(object sender, EventArgs e)
        {

        }

        private void DsingleDepth_KeyPress(object sender, KeyPressEventArgs e)
        {
           // Char aa = e.KeyChar;
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == ',') || e.KeyChar == '\b' || (e.KeyChar == '.'))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '，')
            {
                e.KeyChar = ',';
                e.Handled = false;
            }
            else
            {
                MessageBox.Show("请填写'0'到'9'和','！");
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Char aa = e.KeyChar;
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == ',') || e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '，')//将中文逗号替换为英文逗号
            {
                e.KeyChar = ',';
                e.Handled = false;
            }
            else
            {
                MessageBox.Show("请填写'0'到'9'和','！");
                e.Handled = true;
            }
        }

        private void DintervalTM3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (DintervalTM3.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(DintervalTM3.Text, out oldf);
                    b2 = float.TryParse(DintervalTM3.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void DintervalTM4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (DintervalTM4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(DintervalTM4.Text, out oldf);
                    b2 = float.TryParse(DintervalTM4.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void DintervalTM1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (DintervalTM1.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(DintervalTM1.Text, out oldf);
                    b2 = float.TryParse(DintervalTM1.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void DintervalTM2_TextChanged(object sender, EventArgs e)
        {

        }

        private void DintervalTM2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (DintervalTM2.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(DintervalTM2.Text, out oldf);
                    b2 = float.TryParse(DintervalTM2.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void GratDepTM1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (GratDepTM1.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(GratDepTM1.Text, out oldf);
                    b2 = float.TryParse(GratDepTM1.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void GratDepTM2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (GratDepTM2.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(GratDepTM2.Text, out oldf);
                    b2 = float.TryParse(GratDepTM2.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void GratDepTM3_TextChanged(object sender, EventArgs e)
        {

        }

        private void GratDepTM3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (GratDepTM3.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(GratDepTM3.Text, out oldf);
                    b2 = float.TryParse(GratDepTM3.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void GratDepTM4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (GratDepTM4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(GratDepTM4.Text, out oldf);
                    b2 = float.TryParse(GratDepTM4.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void DTSdifMax_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void TextstrNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                //MessageBox.Show("请输入数字");
            }
        }
       
     }
   }



