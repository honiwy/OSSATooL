using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private String connectionLog = "狀態異常!\n請排除硬體問題再進行後續燒機";
        private bool connectionStatus = false;

        private String errorQFIL = "燒機失敗";
        private String successQFIL = "燒機成功";
        private String qfiling = "燒機中";
        private String qfilLog = "狀態異常!\n請確認硬體是否異常\n請確認 USB/網路線是否脫落";
        private bool qfilSuccessful = false;

        private void tabControlMenu_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            txt1ConnectionStatus.Text = connecting;
            btn1ConnectCheck.Enabled = false;
            bgWorkerConnection.RunWorkerAsync();
        }
        private void connectionProcess()
        {
            //todo add script here
            for (int j = 0; j < 100; j++)
            {
                System.Threading.Thread.Sleep(10);
                bgWorkerConnection.ReportProgress(j);
            }
            connectionStatus = true;
        }

        private void bgWorkerConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            connectionProcess();
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
            }
            else
            {
                txt1ConnectionStatus.Text = errorConnection;
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
            for (int j = 0; j < 100; j++)
            {
                System.Threading.Thread.Sleep(5);
                bgWorkerQFIL.ReportProgress(j);
            }
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

    }
}
