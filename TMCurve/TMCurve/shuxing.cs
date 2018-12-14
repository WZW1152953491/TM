using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TMCurve
{
    public partial class shuxing : Form
    {
        public static string aa = "22s";
        string txt;

        public string Txt
        {

            set { txt = value; }

        }
       
        private static shuxing _instance;
        public shuxing()
        {
            _instance = this;
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//线程就能安全的访问窗体控件了//该方法是通过采用取消线程安全保护模式的方式实现的，所以不建议采用。

        }

        private static readonly object obj = new object();
        public static shuxing getInstance()
        {
            if (null == shuxing._instance)
              {
                  lock (obj)
                  {
                      if (null == shuxing._instance)
                      {
                          shuxing._instance = new shuxing();
                      }
                  }
 
              }
            return shuxing._instance;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // int adad = MainForm.num;
           // string aa=MainForm.getInstance().YDTS3.Text;
           //// MainForm.getInstance().YDTS3.Text = "asdasdasdasd";//可以实现
           // MessageBox.Show(aa+" ");
            //MyClass.drawingzgcTime.DrawingTime();//可以实现

         
            MyClass.drawAttribute.baocunAttribute();
            MessageBox.Show("保存成功！部分设置需要重新生成图像!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void shuxing_Load(object sender, EventArgs e)
        {
             int num = MainForm.getInstance().tabControl1.SelectedIndex;//页面的编码
            // string str=  MyClass.drawAttribute.getAttribute(num,"C:\\Users\\wangzhiwei\\Desktop\\www.txt");
            shuxing.getInstance().sxXtitle.Enabled = true;
            shuxing.getInstance().sxYtitle.Enabled = true;
            shuxing.getInstance().Line.Enabled = true;
            shuxing.getInstance().notLine.Enabled = true;
            if (MyClass.drawAttribute.Linenum == 1)
            {
                Line.Checked = true;
            }
            else
            {
                notLine.Checked = true;
            }
            if (num == 5||num==6||num==4)
            {
                shuxing.getInstance().Line.Enabled = false;
                shuxing.getInstance().notLine.Enabled = false;
            }
           
            shuxing.getInstance().Wellzero.Text = MyClass.drawAttribute.wellZero.ToString();
            if (num == 0)
            {
                //ZedGraph.GraphPane gpDTS1 = MainForm.getInstance().zgcDep.GraphPane;
                if (MyClass.drawAttribute.DDtitle == null)
                {
                  shuxing.getInstance().sxtitle.Text= "LD27-2平台 A22H井 DTS时间温度曲线"; //图表轴名称
                }
                else
                {
                    
                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.DDtitle;
                }
                if (MyClass.drawAttribute.DDXtitle == null)
                {
                    shuxing.getInstance().sxXtitle.Text = "时间"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxXtitle.Text = MyClass.drawAttribute.DDXtitle;
                }
                if (MyClass.drawAttribute.DDYtitle == null)
                {
                    shuxing.getInstance().sxYtitle.Text = "温度（℃）"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxYtitle.Text = MyClass.drawAttribute.DDYtitle;
                }
            }
            else if (num == 1)
            {
                //ZedGraph.GraphPane gpDTS2 = MainForm.getInstance().zgcTime.GraphPane;
                if (MyClass.drawAttribute.DTtitle == null)
                {
                    shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 DTS温度深度曲线"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.DTtitle;
                }
                if (MyClass.drawAttribute.DTXtitle == null)
                {
                    shuxing.getInstance().sxXtitle.Text = "深度(m)"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxXtitle.Text = MyClass.drawAttribute.DTXtitle;
                }
                if (MyClass.drawAttribute.DTYtitle == null)
                {
                    shuxing.getInstance().sxYtitle.Text = "温度（℃）"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxYtitle.Text = MyClass.drawAttribute.DTYtitle;
                }
            }
            else if (num == 2)
            {
                //ZedGraph.GraphPane gpDTS1 = MainForm.getInstance().zgcDep.GraphPane;
                if (MyClass.drawAttribute.GDtitle == null)
                {
                    shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 光栅时间温度曲线"; //图表轴名称
                }
                else
                {
                    
                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.GDtitle;
                }
                if (MyClass.drawAttribute.GDXtitle == null)
                {
                    shuxing.getInstance().sxXtitle.Text = "时间"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxXtitle.Text = MyClass.drawAttribute.GDXtitle;
                }
                if (MyClass.drawAttribute.GDYtitle == null)
                {
                    shuxing.getInstance().sxYtitle.Text = "温度（℃）"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxYtitle.Text = MyClass.drawAttribute.GDYtitle;
                }
            }
            else if (num == 3)
            {
                //ZedGraph.GraphPane gpDTS2 = MainForm.getInstance().zgcTime.GraphPane;
                if (MyClass.drawAttribute.GTtitle == null)
                {
                    shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 光栅温度深度曲线"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.GTtitle;
                }
                if (MyClass.drawAttribute.GTXtitle == null)
                {
                    shuxing.getInstance().sxXtitle.Text = "深度(m)"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxXtitle.Text = MyClass.drawAttribute.GTXtitle;
                }
                if (MyClass.drawAttribute.GTYtitle == null)
                {
                    shuxing.getInstance().sxYtitle.Text = "温度（℃）"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxYtitle.Text = MyClass.drawAttribute.GTYtitle;
                }
            }
            else if (num == 4)
            {
                shuxing.getInstance().sxXtitle.Enabled = false;
                shuxing.getInstance().sxYtitle.Enabled = false;
                //ZedGraph.GraphPane gpDTS2 = MainForm.getInstance().zgcTime.GraphPane;
                if (MyClass.drawAttribute.Welltitle == null)
                {
                    shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 管柱图"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.Welltitle;
                }
          
            }
            else if (num ==5)
            {
                //ZedGraph.GraphPane gpDTS1 = MainForm.getInstance().zgcDep.GraphPane;
                if (MyClass.drawAttribute.realDTtitle == null)
                {
                    shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 DTS深度温度曲线"; //图表轴名称
                }
                else
                {

                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.realDTtitle;
                }
                if (MyClass.drawAttribute.realDTXtitle == null)
                {
                    shuxing.getInstance().sxXtitle.Text = "深度(m)"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxXtitle.Text = MyClass.drawAttribute.realDTXtitle;
                }
                if (MyClass.drawAttribute.realDTYtitle == null)
                {
                    shuxing.getInstance().sxYtitle.Text = "温度（℃）"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxYtitle.Text = MyClass.drawAttribute.realDTYtitle;
                }
            }
            else if (num == 6)
            {
                //ZedGraph.GraphPane gpDTS1 = MainForm.getInstance().zgcDep.GraphPane;
                if (MyClass.drawAttribute.realGTtitle == null)
                {
                    shuxing.getInstance().sxtitle.Text = "LD27-2平台 A22H井 光栅深度温度曲线"; //图表轴名称
                }
                else
                {

                    shuxing.getInstance().sxtitle.Text = MyClass.drawAttribute.realGTtitle;
                }
                if (MyClass.drawAttribute.realGTXtitle == null)
                {
                    shuxing.getInstance().sxXtitle.Text = "深度(m)"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxXtitle.Text = MyClass.drawAttribute.realGTXtitle;
                }
                if (MyClass.drawAttribute.realGTYtitle == null)
                {
                    shuxing.getInstance().sxYtitle.Text = "温度（℃）"; //图表轴名称
                }
                else
                {
                    shuxing.getInstance().sxYtitle.Text = MyClass.drawAttribute.realGTYtitle;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyClass.drawAttribute.morenAttribute();
            //MessageBox.Show("已经恢复默认，请点击保存！");
            MessageBox.Show("已经恢复默认，请点击保存！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Wellzero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (Wellzero.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(Wellzero.Text, out oldf);
                    b2 = float.TryParse(Wellzero.Text + e.KeyChar.ToString(), out f);
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

        private void Line_CheckedChanged(object sender, EventArgs e)
        {
            //if (Line.Checked == true)
            //{
            //    notLine.Checked = false;
            //}
            //else
            //{
            //    Line.Checked = true;
            //}
        }

        private void notLine_CheckedChanged(object sender, EventArgs e)
        {
            //if (notLine.Checked == true)
            //{
            //   Line.Checked = false;
            //}
            //else
            //{
            //    notLine.Checked = true;
            //}
        }
    }
}
