using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;


namespace ossaTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private String error = "失敗 :(";
        private String success = "成功 :)";

        private String connecting = "連線中";
        private String connectionErrLog = "狀態異常!\r\n請排除硬體問題再進行後續燒機";
        private bool connectionStatus = false;
        private String adbLog = "";

        private String rebootEdling = "切換中";
        private String rebootEdlErrLog = "切換異常!\r\n請排除硬體問題再進行後續燒機";
        private bool rebootEdlStatus = false;
        private String rebootEdlSuccessLog = "切換成功!\r\n可進行後續燒機";

        private String qfiling = "燒機中";
        private String qfilErrLog = "狀態異常!\r\n請確認硬體是否異常\r\n請確認 USB/網路線是否脫落";
        private bool qfilSuccessful = false;

        //@"D:\OSSA_new\ossaTool\adb\adb.exe";
        private String adbPath = Path.Combine(Path.GetPathRoot(Application.StartupPath), "OSSA_new\\ossaTool\\adb\\adb.exe");
        //@"D:\OSSA_new\ossaTool\flash_images_and_validate.bat";
        private String qfilPath = Path.Combine(Path.GetPathRoot(Application.StartupPath), "OSSA_new\\ossaTool\\flash_images_and_validate.bat");

        private void button1_Click(object sender, EventArgs e)
        {
            txt1ConnectionStatus.Text = connecting;
            btn1ConnectCheck.Enabled = false;
            connectionStatus = false;
            bgWorkerConnection.RunWorkerAsync();
            txtLog.Text = "Still checking....\r\n";
        }

        private void bgWorkerConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            connectionProcess(e,false);
        }

        private void connectionProcess(DoWorkEventArgs e, bool edlProcess)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = adbPath;
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
                adbLog = p.StandardOutput.ReadToEnd();
                p.Close();
                if (rgx.Matches(adbLog).Count == 1 && edlProcess)
                {
                    bgWorkerEdl.ReportProgress(100);
                    rebootEdlStatus = true;
                    e.Cancel = true;
                    break;

                }
                else if( rgx.Matches(adbLog).Count > 1 && !edlProcess)
                {
                    bgWorkerConnection.ReportProgress(100);
                    connectionStatus = true;
                    e.Cancel = true;
                    break;
                }
                Thread.Sleep(300);
                if (edlProcess) {
                    bgWorkerEdl.ReportProgress(j);
                }
                else {
                    bgWorkerConnection.ReportProgress(j);
                }
            }
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
                txt1ConnectionStatus.Text = success;
                btnEdl.Enabled = true;
                txtLog.Text = adbLog;
            }
            else
            {
                txt1ConnectionStatus.Text = error;
                btnEdl.Enabled = false;
                txtLog.Text = connectionErrLog;
            }
        }

        private void btnEdl_Click(object sender, EventArgs e)
        {
            txt1EdlStatus.Text = rebootEdling;
            btnEdl.Enabled = false;
            rebootEdlStatus = false;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = adbPath;
            psi.Arguments = "reboot edl";
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            p.StandardInput.WriteLine("adb reboot edl");
            p.Close();
            bgWorkerEdl.RunWorkerAsync();
        }

        private void bgWorkerEdl_DoWork(object sender, DoWorkEventArgs e)
        {
            connectionProcess(e,true);
        }

        private void bgWorkerEdl_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarEdl.Value = e.ProgressPercentage;
        }

        private void bgWorkerEdl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnEdl.Enabled = true;
            if (rebootEdlStatus)
            {
                txt1EdlStatus.Text = success;
                btnQFIL.Enabled = true;
                txtLog.Text = rebootEdlSuccessLog;
            }
            else
            {
                txt1EdlStatus.Text = error;
                btnQFIL.Enabled = false;
                txtLog.Text = rebootEdlErrLog;
            }
        }

        private void btnQFIL_Click(object sender, EventArgs e)
        {
            txt1QFILStatus.Text = qfiling;
            btnQFIL.Enabled = false;
            bgWorkerQFIL.RunWorkerAsync();
        }

        private void qFILProcess(DoWorkEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = qfilPath;
            //Fixme : should let user select qfil folder
            //p.StartInfo.ArgumentList.Add("-Mode=3");
            //p.StartInfo.ArgumentList.Add("-downloadflat");
            //p.StartInfo.ArgumentList.Add("-COM=7");
            //p.StartInfo.ArgumentList.Add("-Programmer=true;\"\"\"C:\\Users\\avc\\Desktop\\Cabello_SR2_2021_05_06_66_release_Flat\\emmc\\prog_firehose_ddr.elf\"\"\"");
            //p.StartInfo.ArgumentList.Add("-deviceType=\"\"\"emmc\"\"\"");
            //p.StartInfo.ArgumentList.Add("-VALIDATIONMODE=2");
            //p.StartInfo.ArgumentList.Add("-SWITCHTOFIREHOSETIMEOUT=50");
            //p.StartInfo.ArgumentList.Add("-RESETTIMEOUT=500");
            //p.StartInfo.ArgumentList.Add("-RESETDELAYTIME=5");
            //p.StartInfo.ArgumentList.Add("-RESETAFTERDOWNLOAD=true");
            //p.StartInfo.ArgumentList.Add("-MaxPayloadSizeToTargetInBytes=true;49152");
            //p.StartInfo.ArgumentList.Add("-searchpath=\"\"\"C:\\Users\\avc\\Desktop\\Cabello_SR2_2021_05_06_66_release_Flat\\emmc\"\"\"");
            //p.StartInfo.ArgumentList.Add("-Rawprogram=\"\"\"rawprogram_unsparse.xml\"\"\"");
            //p.StartInfo.ArgumentList.Add("-Patch=\"\"\"patch0.xml\"\"\"");
            //p.StartInfo.ArgumentList.Add("-logfilepath=\"\"\"C:\\QFIL\\log\\flat_log.txt\"\"\"");
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.Close();
            for (int j = 0; j < 100; j++)
            {
                Thread.Sleep(1800);
                bgWorkerQFIL.ReportProgress(j);
                if (qfilSuccessful)
                {
                    bgWorkerQFIL.ReportProgress(100);
                    e.Cancel = true;
                    break;
                }
                    
            }
        }

        private void bgWorkerQFIL_DoWork(object sender, DoWorkEventArgs e)
        {
            qFILProcess(e);
        }

        private void bgWorkerQFIL_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarQFIL.Value = e.ProgressPercentage;

            Regex rgx = new Regex("Finish Download");
            String log = File.ReadAllText("C:\\QFIL\\log\\flat_log.txt");
            txtLog.Text = log;
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
            if (rgx.Matches(log).Count == 1)
                qfilSuccessful = true;
        }

        private void bgWorkerQFIL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnQFIL.Enabled = true;
            if (qfilSuccessful)
            {
                txt1QFILStatus.Text = success;
            }
            else
            {
                txt1QFILStatus.Text = error;
                txtLog.Text = qfilErrLog;
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
