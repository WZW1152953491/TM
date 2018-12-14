using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TMCurve.MyClass
{
    class DataBaseDo
    {
        public static void backupDataBase()
        {
            try
            {
                //构建执行的命令
                StringBuilder sbcommand = new StringBuilder();
                StringBuilder sbfileName = new StringBuilder();
                sbfileName.AppendFormat("{0}", DateTime.Now.ToString()).Replace("-", "").Replace(":", "").Replace(" ", "");
                String fileName = sbfileName.ToString();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = false;
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.CheckPathExists = false;
                saveFileDialog.FileName = fileName;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    String directory = saveFileDialog.FileName;
                    sbcommand.AppendFormat("mysqldump --quick --host=localhost --default-character-set=gbk --lock-tables --verbose  --force --port=3306 --user=root --password=120902 test -r \"{0}\"", directory);
                    String command = sbcommand.ToString();

                    //获取mysqldump.exe所在路径
                    String appDirecroty = System.Windows.Forms.Application.StartupPath + "\\";
                    StartCmd(appDirecroty, command);
                    MainForm.getInstance().DataDo.Enabled = true;
                    MessageBox.Show(@"数据库已成功备份到 " + directory + " 文件中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MainForm.getInstance().DataDo.Enabled = true;
                MessageBox.Show("数据库备份失败！" + ex);

            }
            
        }
       public static void reductionDataBase()
        {
            try
            {
                StringBuilder sbcommand = new StringBuilder();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    String directory = openFileDialog.FileName;
                    //在文件路径后面加上""避免空格出现异常
                    sbcommand.AppendFormat("mysql --host=localhost --default-character-set=gbk --port=3306 --user=root --password=120902 test<\"{0}\"", directory);
                    String command = sbcommand.ToString();
                    //获取mysql.exe所在路径
                    String appDirecroty = System.Windows.Forms.Application.StartupPath + "\\";
                    DialogResult result = MessageBox.Show("您是否真的想覆盖以前的数据库吗？那么以前的数据库数据将丢失！！！", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        StartCmd(appDirecroty, command);
                        MainForm.getInstance().DataDo.Enabled = true;
                        MessageBox.Show("数据库还原成功！");
                    }
                }

            }
            catch (Exception ex)
            {
                MainForm.getInstance().DataDo.Enabled = true;
                MessageBox.Show("数据库还原失败！" + ex);
            }
           
        }
      public static void StartCmd(String workingDirectory, String command)
      {
          Process p = new Process();
          p.StartInfo.FileName = "cmd.exe";
          p.StartInfo.WorkingDirectory = workingDirectory;
          p.StartInfo.UseShellExecute = false;
          p.StartInfo.RedirectStandardInput = true;
          p.StartInfo.RedirectStandardOutput = true;
          p.StartInfo.RedirectStandardError = true;
          p.StartInfo.CreateNoWindow = true;
          p.Start();
          p.StandardInput.WriteLine(command);
          p.StandardInput.WriteLine("exit");
      }
    }
}
