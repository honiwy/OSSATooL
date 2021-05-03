using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ossaTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private String errorConnection = "連線失敗";
        private String successConnection = "連線正常";
        private String connecting = "連線中";
        private String connectionLog = "狀態異常!\r\n請排除硬體問題再進行後續燒機";
        private bool connectionStatus = false;
        private String adbLog = "";

        private String errorQFIL = "燒機失敗";
        private String successQFIL = "燒機成功";
        private String qfiling = "燒機中";
        private String qfilLog = "狀態異常!\r\n請確認硬體是否異常\r\n請確認 USB/網路線是否脫落";
        private bool qfilSuccessful = false;

        private void button1_Click(object sender, EventArgs e)
        {
            txt1ConnectionStatus.Text = connecting;
            btn1ConnectCheck.Enabled = false;
            connectionStatus = false;
            bgWorkerConnection.RunWorkerAsync();
            txtLog.Text = "Still checking....\r\n";
        }

        private void connectionProcess(DoWorkEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "adb.exe";
            psi.Arguments = "devices";
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Regex rgx = new Regex("device");
            for (int j = 0; j < 100; j++)
            {
                Process p = new Process();
                p.StartInfo = psi;
                p.Start();
                p.StandardInput.WriteLine("adb devices");
                String result = p.StandardOutput.ReadToEnd();

                p.Close();
                if (rgx.Matches(result).Count>1)
                {
                    adbLog = result;
                    bgWorkerConnection.ReportProgress(100);
                    connectionStatus = true;
                    e.Cancel = true;
                    break;
                }
                System.Threading.Thread.Sleep(300);
                bgWorkerConnection.ReportProgress(j);
            }
        }

        private void bgWorkerConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            connectionProcess(e);
        }

        private void bgWorkerConnection_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarConnection.Value = e.ProgressPercentage;
        }

        private void bgWorkerConnection_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn1ConnectCheck.Enabled = true;
            if (connectionStatus)
            {
                txt1ConnectionStatus.Text = successConnection;
                btnQFIL.Enabled = true;
                txtLog.Text = adbLog;
            }
            else
            {
                txt1ConnectionStatus.Text = errorConnection;
                btnQFIL.Enabled = false;
                txtLog.Text = connectionLog;
            }
        }
        private void btnQFIL_Click(object sender, EventArgs e)
        {
            txt1QFILStatus.Text = qfiling;
            btnQFIL.Enabled = false;
            bgWorkerQFIL.RunWorkerAsync();
        }

        private void qFILProcess()
        {
            //todo add script here
            //for (int j = 0; j < 100; j++)
            //{
            //    System.Threading.Thread.Sleep(5);
            //    bgWorkerQFIL.ReportProgress(j);
            //}
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.FileName = "D://QFIL_Helper/flash_images_and_validate.bat"; 
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.Start();
            qfilSuccessful = true;
        }


        private void bgWorkerQFIL_DoWork(object sender, DoWorkEventArgs e)
        {
            qFILProcess();
        }

        private void bgWorkerQFIL_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarQFIL.Value = e.ProgressPercentage;
        }

        private void bgWorkerQFIL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnQFIL.Enabled = true;
            if (qfilSuccessful)
            {
                txt1QFILStatus.Text = successQFIL;
            }
            else
            {
                txt1QFILStatus.Text = errorQFIL;
                txtLog.Text = qfilLog;
            }
        }

        private void adbCommand()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c adb connect 172.16.116.34";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.Start();
        }

    }
}
