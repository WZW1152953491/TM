using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMCurve.MyClass
{
    class setGet
    {
        //public static String DsingleDepth;//单项深度
        //public static int DintervalDepth1;//区间深度1
        //public static int DintervalDepth2;//区间深度2
        //public static float DintervalTM1;//区间温度1
        //public static float DintervalTM2;//区间温度2
        private  DateTime dintervalTime1;
        private DateTime dintervalTime2 = MainForm.getInstance().DintervalTime2.Value;
        public DateTime DintervalTime1
       {
            get { return dintervalTime1; }
            set { dintervalTime1 =MainForm.getInstance().DintervalTime1.Value; }
       }
    }
}
