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
            _p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _adbPath,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
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

            comboBox1.Items.Add("BST_BULLET");
            comboBox1.Items.Add("BST_VD2");
            comboBox1.Items.Add("BST_MD2");
            comboBox1.Items.Add("Generic");
            comboBox1.Text = Properties.Settings.Default.SkuIdSaveInfo;
        }

        private string _adbPath = "adb.exe";
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
        private string _skuId = "";
        private string _key = "";
        private bool _keyExisted = false;
        private bool _provisionFinished = false;
        private string _keyboxPath = "";
        private readonly Process _p;

        #region tabpage 1
        private void btn1Connect_Click(object sender, EventArgs e)
        {
            txt1ConnectionStatus.Text = _pleaseWait;
            btn1ConnectCheck.Enabled = false;
            _connectionStatus = false;
            bgWorkerConnection.RunWorkerAsync();
        }

        private void bgWorkerConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            ConnectionProcess(e, false);
        }

        private void ConnectionProcess(DoWorkEventArgs e, bool edlProcess)
        {
            Regex rgx = new Regex("device");
            for (int j = 0; j < 100; j++)
            {
                _p.StartInfo.Arguments = "devices";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _processLog = _p.StandardOutput.ReadToEnd();
                _p.Close();
                if (rgx.Matches(_processLog).Count == 1 && edlProcess)
                {
                    bgWorkerEdl.ReportProgress(100);
                    _rebootEdlStatus = true;
                    e.Cancel = true;
                    break;

                }
                else if (rgx.Matches(_processLog).Count > 1 && !edlProcess)
                {
                    bgWorkerConnection.ReportProgress(100);
                    _connectionStatus = true;
                    e.Cancel = true;
                    break;
                }
                Thread.Sleep(300);
                if (edlProcess)
                {
                    bgWorkerEdl.ReportProgress(j);
                }
                else
                {
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
            if (_connectionStatus)
            {
                txt1ConnectionStatus.Text = _success;
                btnEdl.Enabled = true;
                txtLog.Text = _processLog;
            }
            else
            {
                txt1ConnectionStatus.Text = _error;
                btnEdl.Enabled = false;
                txtLog.Text = _errLog;
            }
        }

        private void btnEdl_Click(object sender, EventArgs e)
        {
            txt1EdlStatus.Text = _pleaseWait;
            btnEdl.Enabled = false;
            _rebootEdlStatus = false;
            _p.StartInfo.Arguments = "reboot edl";
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            _p.Close();
            bgWorkerEdl.RunWorkerAsync();
        }

        private void bgWorkerEdl_DoWork(object sender, DoWorkEventArgs e)
        {
            ConnectionProcess(e, true);
        }

        private void bgWorkerEdl_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarEdl.Value = e.ProgressPercentage;
        }

        private void bgWorkerEdl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnEdl.Enabled = true;
            if (_rebootEdlStatus)
            {
                txt1EdlStatus.Text = _success;
                btnQFIL.Enabled = true;
                txtLog.Text = _rebootEdlSuccessLog;
            }
            else
            {
                txt1EdlStatus.Text = _error;
                btnQFIL.Enabled = false;
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

        private void btnQFIL_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Properties.Settings.Default.QFILFilePath))
            {
                MessageBox.Show("燒機檔案不存在, 請重新確認");
                return;
            }
            if (new FileInfo(Properties.Settings.Default.QFILFilePath).Extension != ".elf")
            {
                MessageBox.Show("燒機檔案格式不正確, 請重新確認");
                return;
            }
            txt1QFILStatus.Text = _pleaseWait;
            btnQFIL.Enabled = false;
            bgWorkerQFIL.RunWorkerAsync();
        }

        private int getCOMPort()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();
            int port = 7;
            if (ports.Length > 0)
            {
                Int32.TryParse(ports[0].Replace("COM", ""), out port);
            }
            return port;

        }

        private void QFILProcess(DoWorkEventArgs e)
        {
            string argument = "-Mode=3 -downloadflat -COM="+ getCOMPort() + "  -Programmer=true;\"" + Properties.Settings.Default.QFILFilePath + "\" -deviceType=\"emmc\" - VALIDATIONMODE=2 " +
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
            if (e.ProgressPercentage > 20 && new Regex("Finish Download").Matches(_processLog).Count == 1 && new Regex("Download Succeed").Matches(_processLog).Count == 1)
                _qfilStatus = true;
        }

        private void bgWorkerQFIL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnQFIL.Enabled = true;
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
                #region get IP address
                _p.StartInfo.Arguments = "root";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _p.Close();

                Thread.Sleep(1500);

                //_p.StartInfo.Arguments = "shell am broadcast -a com.avc.app.fqctool.action.EXEC_CMD -n com.avc.avcfqctool/.receiver.ShellCmdReceiver";
                //_p.Start();
                //_p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                //_p.Close();

                //Thread.Sleep(1500);
                                                    
                // _p.StartInfo.Arguments = "shell cat /sdcard/fqc_file/fqc_app_result.txt";
                //_p.Start();
                //_p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                //_processLog = _p.StandardOutput.ReadToEnd();
                //_p.Close();

                //MatchCollection matchCollection = new Regex(@"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}").Matches(_processLog);
                //txtLog2.Text = (matchCollection.Count > 0) ? "IP Adderesses :[" + matchCollection[0].ToString() + "]" : "請拔除USB線後靜待 5秒再重新測試";

                _p.StartInfo.Arguments = "logcat -d | findstr LinkAddresses";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _processLog = _p.StandardOutput.ReadToEnd();
                _p.Close();
                MatchCollection matchCollection = new Regex(@"LinkAddresses.+?(?=\])").Matches(_processLog);
                txtLog2.Text = (matchCollection.Count > 0) ? matchCollection[0].ToString() : "請拔除USB線後靜待 5秒再重新測試";
                #endregion

                #region get device serial number
                _p.StartInfo.Arguments = "shell cat /avc_info/device_sn";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _deviceSerialNumber = _p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
                _p.Close();
                txtLog2.Text += Environment.NewLine + "Serial number: [" + _deviceSerialNumber + "]";
                #endregion

                #region get device mac address
                _p.StartInfo.Arguments = "shell cat /avc_info/mac_address";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _macAddress = _p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
                _p.Close();
                txtLog2.Text += Environment.NewLine + "MAC address: [" + _macAddress + "]";
                #endregion

                #region write sku ID

                string sku_id = Properties.Settings.Default.SkuIdSaveInfo;

                _p.StartInfo.Arguments = "shell echo '" + sku_id + "' > /avc_info/sku_id";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _p.Close();
                #endregion

                #region get device sku ID
                _p.StartInfo.Arguments = "shell cat /avc_info/sku_id";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _skuId = _p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
                _p.Close();
                txtLog2.Text += Environment.NewLine + "Sku ID : [" + _skuId + "]";
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
            else
            {
                MessageBox.Show("Oh no! 接回 USB 線才能取得相關訊息");
            }
        }

        private void btnRPMBInitialize_Click(object sender, EventArgs e)
        {
            _provisionFinished = false;

            _p.StartInfo.Arguments = "shell echo '1' | qseecom_sample_client v smplap32 14 1";
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            txtLog2.Text = _p.StandardOutput.ReadToEnd();
            _p.Close();

            _p.StartInfo.Arguments = "shell echo '2' | qseecom_sample_client v smplap32 14 1";
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            txtLog2.Text += _p.StandardOutput.ReadToEnd();
            _p.Close();

            txtLog2.SelectionStart = txtLog2.TextLength;
            txtLog2.ScrollToCaret();

            if (new Regex("RPMB_KEY_PROVISIONED_AND_OK").Matches(txtLog2.Text).Count == 1)
            {
                _p.StartInfo.Arguments = "reboot";
                _p.Start();
                _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
                _p.Close();
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

            _p.StartInfo.Arguments = "shell getprop ro.product.board";
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            string deviceType = _p.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
            _p.Close();
            if (!deviceType.Equals("qcs605") && !deviceType.Equals("qcs603"))
            {
                MessageBox.Show("目前不支援 " + deviceType + "裝置種類, 請聯繫客服!");
                return;
            }

            _p.StartInfo.Arguments = "root";
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            _p.Close();

            Thread.Sleep(1500);

            _p.StartInfo.Arguments = "push " + _keyboxPath + " /data";
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            txtLog2.Text = _p.StandardOutput.ReadToEnd();
            _p.Close();

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            switch ((string)comboBox1.SelectedItem) {
                case "BST_BULLET":
                    Properties.Settings.Default.SkuIdSaveInfo = "BST_BULLET";
                    Properties.Settings.Default.Save();
                    break;
                case "BST_VD2":
                    Properties.Settings.Default.SkuIdSaveInfo = "BST_DOME";
                    Properties.Settings.Default.Save();
                    break;
                case "BST_MD2":
                    Properties.Settings.Default.SkuIdSaveInfo = "BST_MD2";
                    Properties.Settings.Default.Save();
                    break;
                case "Generic":
                    Properties.Settings.Default.SkuIdSaveInfo = "Generic";
                    Properties.Settings.Default.Save();
                    break;
                default:
                    Properties.Settings.Default.SkuIdSaveInfo = "Generic";
                    Properties.Settings.Default.Save();
                    break;

            }
        }
    }
}
