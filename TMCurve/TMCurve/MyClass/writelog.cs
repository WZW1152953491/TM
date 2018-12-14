using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace TMCurve.MyClass
{
    class writelog
    {
        public static void EditFile(int curLine, string newLineValue, string patch)
        {
            FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Read);//创建写入文件 
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            string line = sr.ReadLine();
            StringBuilder sb = new StringBuilder();
            for (int i = 1; line != null; i++)
            {
                sb.Append(line + "\r\n");
                if (i != curLine - 1)
                    line = sr.ReadLine();
                else
                {
                    sr.ReadLine();
                    line = newLineValue;
                }
            }
            sr.Close();
            fs.Close();
            FileStream fs1 = new FileStream(patch, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1);
            sw.Write(sb.ToString());
            sw.Close();
            fs.Close();
        }  

        //public static void WriteLog(string log)
        //{
        //    DateTime DT = System.DateTime.Now;
        //    string ds = DT.ToString("yyyyMMddHHmmss");
        //    string day = System.DateTime.Now.ToString("yyyyMMdd");
        //    double dd = Convert.ToDouble(ds);
        //    string filename = "D:\\"+day+".txt";
        //    //filename = day;
        //    if (!File.Exists(filename))
        //    {
        //        FileStream fs1 = new FileStream(filename, FileMode.Create, FileAccess.Write);//创建写入文件 
        //        StreamWriter sw = new StreamWriter(fs1);
        //        //  sw.WriteLine(this.receiveMsg01.Text.Trim() + "+" + this.receiveMsg01.Text);//开始写入值
        //        sw.WriteLine("时间"+DT+"\n");//开始写入值
        //        sw.WriteLine(log+"\n");//开始写入值
        //        sw.Close();
        //        fs1.Close();
        //    }
        //    else
        //    {
        //        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Write);
        //        StreamWriter sr = new StreamWriter(fs);
        //        sr.WriteLine("时间" + DT + "\n");//开始写入值
        //        sr.WriteLine(log + "\n");//开始写入值
        //        sr.Close();
        //        fs.Close();
        //    }
        //}
        //调试程序总是会用需要一个日志文件记录调试过程。这个代码会自动创建一个文本文件然后在尾行添加新的内容。

        //可能会存在的问题是：如果这个日志已经被一个用户打开，可能其他用户就不能写入了。不过我用了

        //using (StreamWriter SW = File.AppendText(LogFile))来解决这个问题。但是没有进行完全性的测试。


        public static void WriteLog(string Log)
        {
            try
            {
                DateTime DT = System.DateTime.Now;
                string ds = DT.ToString("yyyyMMddHHmmss");
                string day = System.DateTime.Now.ToString("yyyyMMdd");
                double dd = Convert.ToDouble(ds);
                string LogFile = "E:\\" + day + ".txt";
                if (File.Exists(LogFile))
                {
                    WriteLog(Log, LogFile);
                }

                else
                {
                    CreateLog(LogFile);
                    WriteLog(Log, LogFile);
                }
            }
            catch
            {
                MessageBox.Show("日志路径选择不正确，请重新设定。");
            }

        }

        private static void CreateLog(string LogFile)
        {
            StreamWriter SW;
            SW = File.CreateText(LogFile);
            SW.WriteLine("Log created at: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            SW.Close();
        }

        private static void WriteLog(string Log, string LogFile)
        {
            using (StreamWriter SW = File.AppendText(LogFile))
            {
                SW.WriteLine("New Log --" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+ ":"+Log );
                SW.Close();
            }
        }
    }
    
}
