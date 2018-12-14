using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections;
using ZedGraph;
using System.Threading;
using System.Threading.Tasks;

namespace TMCurve.MyClass
{
    class shili
    {
        private static  ZedGraph.GraphPane gp;

        public static ZedGraph.GraphPane Getgp()
        {
                if(gp==null)
                {
                    gp = MainForm.getInstance().zgcDep.GraphPane; 
                }
                return gp;
        }
    }
}
