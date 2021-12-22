using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;

namespace ossaTool
{
    static class Util
    {
        public static string TrimString(string inputString)
        {
            return inputString.Replace("\r\n", string.Empty);
        }

        public static int GetCOMPort()
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

        public static bool CheckQFilSuccessful(string log)
        {
            return new Regex("Finish Download").Matches(log).Count == 1 && new Regex("Download Succeed").Matches(log).Count == 1;
        }

        public static bool CheckRPMBSuccessful(string log)
        {
            return new Regex("RPMB_KEY_PROVISIONED_AND_OK").Matches(log).Count == 1;
        }

        public static string GetIPAddress(string log)
        {
            MatchCollection mc = new Regex(@"LinkAddresses.+?(?=\])").Matches(log);
            if (mc.Count > 0)
            {
                MatchCollection mc2 = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Matches(mc[0].ToString());
                if (mc2.Count > 0)
                {
                    return mc2[0].ToString();
                }
                else{
                    return "";
                }
            }
            else
                return "";
        }

    }
}
