using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Application = System.Windows.Forms.Application;

namespace ossaTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            txtStoragePath.Text = Properties.Settings.Default.FileStoragePath;
            p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }

        //@"D:\OSSA_new\ossaTool\adb\adb.exe";
        private string adbPath = Path.Combine(Path.GetPathRoot(Application.StartupPath), "OSSA_new\\ossaTool\\adb\\adb.exe");
        //@"D:\OSSA_new\ossaTool\flash_images_and_validate.bat";
        private string qfilPath = Path.Combine(Path.GetPathRoot(Application.StartupPath), "OSSA_new\\ossaTool\\flash_images_and_validate.bat");

        private string error = "失敗 :(";
        private string success = "成功 :)";

        private string connecting = "連線中";
        private string connectionErrLog = "狀態異常!\r\n請排除硬體問題再進行後續燒機";
        private bool connectionStatus = false;
        private string adbLog = "";

        private string rebootEdling = "切換中";
        private string rebootEdlErrLog = "切換異常!\r\n請排除硬體問題再進行後續燒機";
        private bool rebootEdlStatus = false;
        private string rebootEdlSuccessLog = "切換成功!\r\n可進行後續燒機";

        private string qfiling = "燒機中";
        private string qfilErrLog = "狀態異常!\r\n請確認硬體是否異常\r\n請確認 USB/網路線是否脫落";
        private bool qfilSuccessful = false;


        private CountDownTimer timer = new CountDownTimer();
        private string device_sn = "";
        private string mac_address = "";
        private string key = "";
        private Process p;

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
            Regex rgx = new Regex("device");
            for (int j = 0; j < 100; j++)
            {
                p.StartInfo.Arguments = "devices";
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
            p.StartInfo.Arguments = "reboot edl";
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
            Process pbat = new Process() { 
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = qfilPath,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                }
            };
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
            pbat.Start();
            pbat.Close();
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
                MessageBox.Show("拔除 USB 線後, 點選 Step 2");
            }
            else
            {
                txt1QFILStatus.Text = error;
                txtLog.Text = qfilErrLog;
            }
        }

        private void adbCommand()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c adb connect 172.16.116.34";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.Start();
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            timer.SetTime(0, 50);
            timer.Start();
            timer.TimeChanged += () => txtCountDown.Text = timer.TimeLeftMsStr;
            timer.StepMs = 77; // for nice milliseconds time switch
        }

        private void tabPage2_Leave(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void btnGetIP_Click(object sender, EventArgs e)
        {
            #region get IP address
            p.StartInfo.Arguments = "logcat -d | findstr LinkAddresses";
            p.Start();
            p.StandardInput.WriteLine("adb logcat -d | findstr LinkAddresses");
            adbLog = p.StandardOutput.ReadToEnd();
            p.Close();
            MatchCollection matchCollection = new Regex(@"LinkAddresses.+?(?=\])").Matches(adbLog);
            txtLog2.Text = (matchCollection.Count > 0)? matchCollection[0].ToString(): "Oops! No IP address got found.";
            #endregion

            #region get device serial number
            p.StartInfo.Arguments = "shell cat /avc_info/device_sn";
            p.Start();
            p.StandardInput.WriteLine("adb shell cat /avc_info/device_sn");
            device_sn = p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
            p.Close();
            txtLog2.Text += Environment.NewLine + "Serial number: [" + device_sn + "]";
            #endregion

            #region get device mac address
            p.StartInfo.Arguments = "shell cat /avc_info/mac_address";
            p.Start();
            p.StandardInput.WriteLine("adb shell cat /avc_info/mac_address");
            mac_address = p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
            p.Close();
            txtLog2.Text += Environment.NewLine + "MAC address: [" + mac_address + "]";
            #endregion

            //if (device_sn.Length != 0 && mac_address.Length != 0)
            //{
                btnUpdateCSV.Enabled = true;
                btnUpdateTXT.Enabled = true;
            //}
        }

        private void btnWriteFile_Click(object sender, EventArgs e)
        {
            string path = Properties.Settings.Default.FileStoragePath + "/deviceInfoLog.txt";
            using (StreamWriter sw = new StreamWriter(path, true))   
            {
                if (sw.BaseStream.Position == 0)
                    sw.WriteLine("Date//Serial #//MAC address//Keybox Dir//Key");
                sw.Write(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "//");
                sw.Write(device_sn + "//");
                sw.Write(mac_address + "//");
                sw.Write("kb007//");
                sw.WriteLine(key);
            }
        }

        private void btnUpdateCSV_Click(object sender, EventArgs e)
        {
            string path = Properties.Settings.Default.FileStoragePath + "/deviceInfoLog.csv";
            using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write)))
            {
                if (sw.BaseStream.Position == 0)
                {
                    sw.WriteLine("sep=,");
                    sw.WriteLine("Date,Serial #,MAC address,Keybox Dir,Key");
                }
                sw.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ","+ device_sn + ","+ mac_address + "," + "kb007," + key);
            }
        }

        private void btnChangePermission_Click(object sender, EventArgs e)
        {
            p.StartInfo.Arguments = "root";
            p.Start();
            p.StandardInput.WriteLine("adb root");
            p.Close();

            p.StartInfo.Arguments = "shell echo '1' | qseecom_sample_client v smplap32 14 1";
            p.Start();
            p.StandardInput.WriteLine("adb shell echo '1' | qseecom_sample_client v smplap32 14 1");
            txtLog2.Text = p.StandardOutput.ReadToEnd();
            p.Close();

            p.StartInfo.Arguments = "shell echo '2' | qseecom_sample_client v smplap32 14 1";
            p.Start();
            p.StandardInput.WriteLine("adb shell echo '2' | qseecom_sample_client v smplap32 14 1");
            txtLog2.Text += p.StandardOutput.ReadToEnd();
            p.Close();

            txtLog2.SelectionStart = txtLog2.TextLength;
            txtLog2.ScrollToCaret();

            Regex rgx = new Regex("RPMB_KEY_PROVISIONED_AND_OK");
            if (rgx.Matches(txtLog2.Text).Count == 1)
               MessageBox.Show("權限已開啟, 可進行後續燒金鑰步驟");
        }

        private void btnKeyBurn_Click(object sender, EventArgs e)
        {
            key = "6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4";
            string keyboxPath = "D:/ossa_cert/kb/kb007/keybox_output/keybox_6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4.xml";
            if (!File.Exists(keyboxPath))
            {
                MessageBox.Show(keyboxPath + "\n 檔案不存在");
                return;
            }

            string keyboxName = Path.GetFileName(keyboxPath);
            if (!keyboxPath.Contains(key))
            {
                MessageBox.Show(key + " 與 keybox 不匹配");
                return;
            }

            p.StartInfo.Arguments = "shell getprop ro.product.board";
            p.Start();
            p.StandardInput.WriteLine("adb shell getprop ro.product.board");
            string deviceType = p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
            p.Close();
            if (!deviceType.Equals("qcs605") && !deviceType.Equals("qcs603"))
            {
                MessageBox.Show("目前不支援 " + deviceType + "裝置種類, 請聯繫客服!");
                return;
            }

            p.StartInfo.Arguments = "root";
            p.Start();
            p.StandardInput.WriteLine("adb root");
            p.Close();

            string arg = "push " + keyboxPath + " /data";
            p.StartInfo.Arguments = arg;
            p.Start();
            p.StandardInput.WriteLine("adb " + arg);
            txtLog2.Text = p.StandardOutput.ReadToEnd();
            p.Close();

            p.StartInfo.Arguments = "root";
            p.Start();
            p.StandardInput.WriteLine("adb root");
            p.Close();

            //string arg2 = "shell LD_LIBRARY_PATH =/vendor/lib64/hw KmInstallKeybox /data/" + keyboxName + " " + key + " true";
            //p.StartInfo.Arguments = "shell LD_LIBRARY_PATH =/vendor/lib64/hw KmInstallKeybox /data/keybox_6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4.xml keybox_6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4 true";
            //p.Start();
            //p.StandardInput.WriteLine("adb shell LD_LIBRARY_PATH =/vendor/lib64/hw KmInstallKeybox /data/keybox_6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4.xml keybox_6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4 true");
            //while (!p.StandardOutput.EndOfStream)
            //{
            //    txtLog2.Text += Environment.NewLine + p.StandardOutput.ReadLine();
            //}
            //txtLog2.Text += Environment.NewLine + arg2;
            //p.Close();

            //p.StartInfo.Arguments = "reboot";
            //p.Start();
            //p.StandardInput.WriteLine("adb reboot");
            //p.Close();
        }

        private void btnChangeStorage_Click(object sender, EventArgs e)
        {
            DialogResult result = storageDirDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                txtStoragePath.Text = storageDirDialog.SelectedPath;
                Properties.Settings.Default.FileStoragePath = storageDirDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
