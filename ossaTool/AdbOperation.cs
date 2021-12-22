using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ossaTool
{
    static class AdbOperation
    {
        private static readonly Process _p;

        static AdbOperation()
        {
            _p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "adb.exe",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }

        public static string PushKeyFile(string keyFilePath)
        {
            return RequestCommand($"push {keyFilePath} /data");
        }

        public static void WriteSkuId()
        {
            RequestCommand($"shell echo '{Properties.Settings.Default.SkuIdSaveInfo}' > /avc_info/sku_id", false, false);
        }

        public static string CheckBoard()
        {
            return RequestCommand("shell getprop ro.product.board");
        }

        public static void RebootDevice()
        {
            RequestCommand("reboot", false, false);
        }

        public static string CheckRPMBStatus()
        {
            return RequestCommand("shell echo '2' | qseecom_sample_client v smplap32 14 1", true, false);
        }

        public static string InitializeRPMB()
        {
            return RequestCommand("shell echo '1' | qseecom_sample_client v smplap32 14 1", true, false);
        }

        public static string CheckSkuId()
        {
            return RequestCommand("shell cat /avc_info/sku_id");
        }

        public static string CheckMacAddress()
        {
            return RequestCommand("shell cat /avc_info/mac_address");
        }

        public static string CheckSerialNumber()
        {
            return RequestCommand("shell cat /avc_info/device_sn");
        }

        public static string LogAddress()
        {
            return RequestCommand("logcat -d | findstr LinkAddresses");
        }

        public static void GiveRootAccess()
        {
            RequestCommand("root", false, false);
        }

        public static void RebootAndEnterEDL()
        {
            RequestCommand("reboot edl", false, false);
        }

        public static string CheckDeviceConnection()
        {
            return RequestCommand("devices", true, false);
        }

        public static string CheckDeviceBrand()
        {
            return RequestCommand("shell getprop ro.product.brand");
        }
        public static string CheckDeviceVersion()
        {
            return RequestCommand("shell getprop ro.build.version.incremental");
        }

        private static string RequestCommand(string adbCommand, bool isReadlineRequired = true, bool isNewlineRemoveRequired = true)
        {
            string output = "";
            _p.StartInfo.Arguments = adbCommand;
            _p.Start();
            _p.StandardInput.WriteLine("adb " + _p.StartInfo.Arguments);
            if (isReadlineRequired)
            {
                output = _p.StandardOutput.ReadToEnd();
                output = isNewlineRemoveRequired ? Util.TrimString(output) : output;
            }
            _p.Close();
            return output;
        }
    }
}
