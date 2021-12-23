using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;

namespace ossaTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadAppInfo();
            LoadSettingValue();
            CheckKey();
        
        }

        private void LoadAppInfo()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            groupBox1.Text = "About " + fvi.ProductName;
            labelDescription.Text = "A total solution for the process of " + Environment.NewLine + "Qualcomm mobile device flash, attestation key provision, and device management.";
            labelAppInfo.Text = fvi.ProductName + " " + fvi.FileVersion + Environment.NewLine + fvi.CompanyName + Environment.NewLine + fvi.LegalCopyright;
        }
        private void LoadSettingValue()
        {
            if (Properties.Settings.Default.FileStoragePath == "")
            {
                Properties.Settings.Default.FileStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Properties.Settings.Default.Save();
            }
            txtStoragePath.Text = Properties.Settings.Default.FileStoragePath;
            _qfilLogPath = $"{Properties.Settings.Default.FileStoragePath}/flat_log.txt";
            txtQFILPath.Text = Properties.Settings.Default.QFILFilePath;
            txtKeyRepoPath.Text = Properties.Settings.Default.KeyRepoPath;
            toggleTXT.IsOn = Properties.Settings.Default.BoolSaveTxt;
            comBoxSku.Text = Properties.Settings.Default.SkuIdSaveInfo;
        }

      
        private string _qfilPath = @"C:\Program Files (x86)\Qualcomm\QPST\bin\qfil.exe";
        private string _qfilLogPath;

        private string _error = "失敗 :(";
        private string _success = "成功 :)";

        private string _pleaseWait = "請稍後...";
        private string _errLog = "狀態異常!\r\n請排除硬體問題或連繫客服";
        private string _processLog = "";
        private bool _connectionStatus = false;
        private bool _rebootEdlStatus = false;
        private bool _qfilStatus = false;
        private string _rebootEdlSuccessLog = "切換成功!\r\n可進行後續燒機";
        private string _qfilErrLog = "狀態異常!\r\n請確認硬體是否異常\r\n請確認 USB 或網路線是否脫落";

        private CountDownTimer _timer = new CountDownTimer();
        private string _deviceSerialNumber = "";
        private string _macAddress = "";
        private string _ipAddress = "";
        private string _skuId = "";
        private string _key = "";
        private bool _keyExisted = false;
        private bool _provisionFinished = false;
        private string _keyboxPath = "";


        #region tabpage 1
        private void btn1Connect_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";

            txt1ConnectionStatus.Text = _pleaseWait;
            panelButtonGroup.Enabled = false;
            _connectionStatus = false;

            bgWorkerConnection.RunWorkerAsync();
        }

        private void bgWorkerConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckProcess(e, false);
        }

        private void CheckProcess(DoWorkEventArgs e, bool edlProcess)
        {
            if (edlProcess)
            {
                for (int checkPoint = 0; checkPoint <= 100; checkPoint++)
                {
                    _processLog = AdbOperation.CheckDeviceConnection();
                    if (Util.CheckDeviceStatus(_processLog) == 1)
                    {
                        bgWorkerEdl.ReportProgress(100);
                        _rebootEdlStatus = true;
                        e.Cancel = true;
                        break;
                    }
                    Thread.Sleep(150);
                    bgWorkerEdl.ReportProgress(checkPoint);
                }
            }
            else
            {
                for (int checkPoint = 0; checkPoint <= 100; checkPoint++)
                {
                    _processLog = AdbOperation.CheckDeviceConnection();
                    if (Util.CheckDeviceStatus(_processLog) > 1)
                    {
                        bgWorkerConnection.ReportProgress(100);
                        _connectionStatus = true;
                        e.Cancel = true;
                        break;
                    }
                    Thread.Sleep(150);
                    bgWorkerConnection.ReportProgress(checkPoint);
                }
            }
        }

        private void bgWorkerConnection_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarConnection.Value = e.ProgressPercentage;
        }

        private void bgWorkerConnection_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panelButtonGroup.Enabled = true;
            if (_connectionStatus)
            {
                txt1ConnectionStatus.Text = _success;
                string deviceType = AdbOperation.CheckBoard();
                txtLog.Text = $"{ _processLog}Board: {deviceType}\r\nBrand: {AdbOperation.CheckDeviceBrand()}\r\nVersion: {AdbOperation.CheckDeviceVersion()}";
                if (!deviceType.Equals("qcs605") && !deviceType.Equals("qcs603"))
                {
                    MessageBox.Show("請更換 qcs605/qcs603 的板子或聯繫客服", $"目前不支援 {deviceType}",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    btnEdl.Enabled = true;
                }
            }
            else
            {
                txt1ConnectionStatus.Text = _error;
                txtLog.Text = _errLog;
            }
        }

        private void btnQFILPath_Click(object sender, EventArgs e)
        {
            qfilFileDialog.Title = "Select file";
            qfilFileDialog.InitialDirectory = ".\\";
            qfilFileDialog.Filter = "elf files (*.*)|*.elf";
            if (qfilFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtQFILPath.Text = qfilFileDialog.FileName;
                txtQFILPath.SelectionStart = txtQFILPath.Text.Length;
                txtQFILPath.SelectionLength = 0;
                Properties.Settings.Default.QFILFilePath = qfilFileDialog.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void btnEdl_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Properties.Settings.Default.QFILFilePath))
            {
                MessageBox.Show("燒機檔案不存在", "請重新確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (new FileInfo(Properties.Settings.Default.QFILFilePath).Extension != ".elf")
            {
                MessageBox.Show("燒機檔案格式不正確", "請重新確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            txt1EdlStatus.Text = _pleaseWait;
            panelButtonGroup.Enabled = false;
            _rebootEdlStatus = false;

            AdbOperation.RebootAndEnterEDL();
            bgWorkerEdl.RunWorkerAsync();
        }

        private void bgWorkerEdl_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckProcess(e, true);
        }

        private void bgWorkerEdl_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarEdl.Value = e.ProgressPercentage;
        }

        private void bgWorkerEdl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panelButtonGroup.Enabled = true;
            if (_rebootEdlStatus)
            {
                txt1EdlStatus.Text = _success;
                btnConnectCheck.Enabled = false;
                btnEdl.Enabled = false;
                btnQFIL.Enabled = true;
                txtLog.Text = _rebootEdlSuccessLog;
            }
            else
            {
                txt1EdlStatus.Text = _error;
                txtLog.Text = _errLog;
            }
        }

        private void btnQFIL_Click(object sender, EventArgs e)
        {
            _processLog = "";
            _qfilStatus = false;
            txt1QFILStatus.Text = _pleaseWait;
            panelButtonGroup.Enabled = false;
            bgWorkerQFIL.RunWorkerAsync();
        }

        private void QFILProcess(DoWorkEventArgs e)
        {
            string argument = "-Mode=3 -downloadflat -COM="+ Util.GetCOMPort() + "  -Programmer=true;\"" + Properties.Settings.Default.QFILFilePath + "\" -deviceType=\"emmc\" - VALIDATIONMODE=2 " +
                "-SWITCHTOFIREHOSETIMEOUT=50 -RESETTIMEOUT=500 -RESETDELAYTIME=5 -RESETAFTERDOWNLOAD=true -MaxPayloadSizeToTargetInBytes=true;49152 -searchpath=\"" +
                Path.GetDirectoryName(Properties.Settings.Default.QFILFilePath) + "\" -Rawprogram=\"rawprogram_unsparse.xml\" -Patch=\"patch0.xml\" -logfilepath=\"" + _qfilLogPath + "\"";

            Process pbat = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = _qfilPath,
                    Arguments = argument,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                }
            };

            pbat.Start();
            pbat.StandardInput.WriteLine("qfil " + argument);
            pbat.Close();
            for (int j = 0; j < 100; j++)
            {
                Thread.Sleep(1800);
                bgWorkerQFIL.ReportProgress(j);
                if (_qfilStatus)
                {
                    bgWorkerQFIL.ReportProgress(100);
                    e.Cancel = true;
                    break;
                }

            }
        }

        private void bgWorkerQFIL_DoWork(object sender, DoWorkEventArgs e)
        {
            QFILProcess(e);
        }

        private void bgWorkerQFIL_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarQFIL.Value = e.ProgressPercentage;
            using (var stream = File.Open(_qfilLogPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                _processLog = File.ReadAllText(_qfilLogPath);
            txtLog.Text = _processLog;
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
            if (e.ProgressPercentage > 20 && Util.CheckQFilSuccessful(_processLog))
                _qfilStatus = true;
        }

        private void bgWorkerQFIL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panelButtonGroup.Enabled = true;
            if (_qfilStatus)
            {
                txt1QFILStatus.Text = _success;
                DialogResult result = MessageBox.Show("拔除 USB 線後, 請依 Step 2 指示操作");
                if (result == DialogResult.OK)
                    tabControlMenu.SelectedIndex = 1;
            }
            else
            {
                txt1QFILStatus.Text = _error;
                txtLog.Text += _qfilErrLog;
            }
        }
        #endregion

        #region tabpage 2
        private void tabPage2_Enter(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.SetTime(0, 50);
            _timer.Start();
            _timer.TimeChanged += () => txtCountDown.Text = _timer.TimeLeftMsStr;
            _timer.StepMs = 77; // for nice milliseconds time switch
        }

        private void btnGetIP_Click(object sender, EventArgs e)
        {
            if(_timer.TimeLeft.TotalSeconds!=0)
            {
                MessageBox.Show("別心急, 倒數還沒結束呢!!");
                return;
            }
            if (MessageBox.Show("USB 線接回去了嗎?", "溫馨提醒", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string currentBrand = "";
                while (currentBrand.Length == 0)
                {
                    Thread.Sleep(2000);
                    currentBrand = AdbOperation.CheckDeviceBrand();
                }

                if (currentBrand != "SnST")
                {
                    AdbOperation.RebootAndEnterEDL();
                    MessageBox.Show($"目前為 {currentBrand} 應為 SnST, 請修正檔案後重新燒錄", "商標有誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControlMenu.SelectedIndex = 0;
                }
                else
                {
                    #region get IP address
                  
                    _processLog = AdbOperation.LogAddress();
                    _ipAddress = Util.GetIPAddress(_processLog);
                    txtLog2.Text = (_ipAddress.Length>0) ? $"IP address: [{_ipAddress}]" : "請拔除USB線後靜待 5秒再重新測試";
                    #endregion

                    #region device serial number
                    _deviceSerialNumber = AdbOperation.CheckSerialNumber();
                    txtLog2.Text += Environment.NewLine + $"Serial number: [{_deviceSerialNumber}]";
                    #endregion

                    #region device mac address
                    _macAddress = AdbOperation.CheckMacAddress();
                    txtLog2.Text += Environment.NewLine + $"MAC address: [{ _macAddress}]";
                    #endregion

                    #region skuID
                    AdbOperation.WriteSkuId();
                    _skuId = AdbOperation.CheckSkuId();
                    txtLog2.Text += Environment.NewLine + $"Sku ID : [{_skuId}]";
                    #endregion

                    if (_deviceSerialNumber.Length != 0 && _macAddress.Length != 0)
                    {
                        btnRPMBInitialize.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("缺少 MAC address 與 序號 的機器無法燒錄金鑰", "錯誤訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Oh no! 接回 USB 線才能取得相關訊息");
            }
        }

        private void comBoxSku_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SkuIdSaveInfo = ((string)comBoxSku.SelectedItem == "BST_VD2") ? "BST_DOME" : (string)comBoxSku.SelectedItem;
            Properties.Settings.Default.Save();
        }

        private void btnRPMBInitialize_Click(object sender, EventArgs e)
        {
            _provisionFinished = false;
            AdbOperation.GiveRootAccess();
            Thread.Sleep(1000);
            AdbOperation.InitializeRPMB();
            txtLog2.Text = AdbOperation.CheckRPMBStatus();
            txtLog2.SelectionStart = txtLog2.TextLength;
            txtLog2.ScrollToCaret();

            if (Util.CheckRPMBSuccessful(txtLog2.Text))
            {
                AdbOperation.RebootDevice();
                _provisionFinished = true;
                MessageBox.Show("RPMB 初始化完成, 待裝置重開機後可繼續燒金鑰步驟");
            }
            CheckKeyBurnEnabled();
        }

        private void btnChangeKeyRepo_Click(object sender, EventArgs e)
        {
            if (keyRepoDialog.ShowDialog() == DialogResult.OK)
            {
                txtKeyRepoPath.Text = keyRepoDialog.SelectedPath;
                txtKeyRepoPath.SelectionStart = txtKeyRepoPath.Text.Length;
                txtKeyRepoPath.SelectionLength = 0;
                Properties.Settings.Default.KeyRepoPath = keyRepoDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
            CheckKey();
        }

        private void CheckKey()
        {
            txtKeyboxPath.Text = Path.GetFileName(Properties.Settings.Default.KeyRepoPath);
            txtKeyName.Text = "無可使用之金鑰";
            if (Directory.Exists(Properties.Settings.Default.KeyRepoPath))
            {
                string[] subKeyboxDirs = Directory.GetDirectories(Properties.Settings.Default.KeyRepoPath);
                _keyExisted = false;
                foreach (string keybox in subKeyboxDirs)
                {
                    string findPath = keybox + @"\keybox_output";
                    if (!Directory.Exists(findPath))
                        continue;

                    IEnumerable<string> files = Directory.GetFiles(findPath, "*.xml", SearchOption.TopDirectoryOnly);
                    Debug.WriteLine("files.Count() " + files.Count());
                    if (files.Count() > 0)
                    {
                        txtKeyboxPath.Text = Path.GetFileName(keybox);
                        string keyFile = Path.GetFileName(files.First());
                        _keyboxPath = files.First();
                        _key = keyFile.Substring(keyFile.IndexOf("_") + 1, 36);
                        txtKeyName.Text = _key;
                        _keyExisted = true;
                        break;
                    }
                }
                CheckKeyBurnEnabled();
            }
        }

        private void CheckKeyBurnEnabled()
        {
            btnKeyBurn.Enabled = (_keyExisted && _provisionFinished);
        }

        private void btnKeyBurn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_keyboxPath))
            {
                MessageBox.Show(_keyboxPath + "\n 檔案不存在");
                return;
            }

            if (!_keyboxPath.Contains(_key))
            {
                MessageBox.Show(_key + " 與 keybox 不匹配");
                return;
            }

    

            AdbOperation.GiveRootAccess();

            Thread.Sleep(1500);

            txtLog2.Text = AdbOperation.PushKeyFile(_keyboxPath);


            if (MessageBox.Show("請開啟命令提示字元並於 Ctrl + V 執行以完成燒機步驟") == DialogResult.OK)
            {
                string time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                string filePrefix = $"{Properties.Settings.Default.FileStoragePath}/deviceInfoLog";
                using (StreamWriter sw = new StreamWriter(new FileStream(filePrefix + ".csv", FileMode.Append, FileAccess.Write)))
                {
                    Debug.WriteLine("save CSV");
                    if (sw.BaseStream.Position == 0)
                    {
                        sw.WriteLine("sep=,");
                        sw.WriteLine("Date,Serial #,MAC address,Keybox Dir,Key");
                    }
                    sw.WriteLine(time + "," + _deviceSerialNumber + "," + _macAddress + "," + txtKeyboxPath.Text + "," + _key);
                }
                if (toggleTXT.IsOn)
                {
                    using StreamWriter sw = new StreamWriter(filePrefix + ".txt", true);
                    if (sw.BaseStream.Position == 0)
                        sw.WriteLine("Date//Serial #//MAC address//Keybox Dir//Key");
                    sw.Write(time + "//" + _deviceSerialNumber + "//" + _macAddress + "//" + txtKeyboxPath.Text + "//");
                    sw.WriteLine(_key);
                }

                string logPath = new DirectoryInfo(Properties.Settings.Default.FileStoragePath + String.Format("/provisioning_log_{0}.txt", _key)).FullName.Replace(@"\", "/");
                string command = String.Format("adb shell \"LD_LIBRARY_PATH=/vendor/lib64/hw KmInstallKeybox /data/{0} {1} true\" > {2}", Path.GetFileName(_keyboxPath), _key, logPath);

                txtLog2.Text += Environment.NewLine + command;
                Clipboard.SetText(command);

                string folder = Path.GetDirectoryName(_keyboxPath) + @"\used\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                File.Move(_keyboxPath, folder + Path.GetFileName(_keyboxPath), true);
                CheckKey();

                //Process.Start(new ProcessStartInfo("http://172.16.116.188") { UseShellExecute = true });
            }
        }
        #endregion

        private void btnChangeStorage_Click(object sender, EventArgs e)
        {
            if (storageDirDialog.ShowDialog() == DialogResult.OK)
            {
                txtStoragePath.Text = storageDirDialog.SelectedPath;
                txtStoragePath.SelectionStart = txtStoragePath.Text.Length;
                txtStoragePath.SelectionLength = 0;
                Properties.Settings.Default.FileStoragePath = storageDirDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void btnOpenStorageDir_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Properties.Settings.Default.FileStoragePath))
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", Properties.Settings.Default.FileStoragePath);
        }

        private void toggleTXT_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BoolSaveTxt = toggleTXT.IsOn;
            Properties.Settings.Default.Save();
        }

      
    }
}
