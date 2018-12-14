using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMCurve.MyClass
{
    class generalClass
    {
        //依照路径获取路径中的时间，格式是“yyyyMMddHHmmss”
        public static DateTime getFileTime(string filename)
        {
            string timestr = filename.Substring(filename.LastIndexOf("\\") + 1, filename.LastIndexOf(".") - (filename.LastIndexOf("\\") + 1));//去除后缀
            string str = timestr.Remove(0, timestr.Length - 14);//取去除后缀的文件名称的后14位数据。
            DateTime dt = DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            return dt;
        }
    }
}
