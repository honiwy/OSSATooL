
namespace ossaTool
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControlMenu = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txt1EdlStatus = new System.Windows.Forms.TextBox();
            this.pgBarEdl = new System.Windows.Forms.ProgressBar();
            this.btnEdl = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.pgBarQFIL = new System.Windows.Forms.ProgressBar();
            this.pgBarConnection = new System.Windows.Forms.ProgressBar();
            this.txt1QFILStatus = new System.Windows.Forms.TextBox();
            this.btnQFIL = new System.Windows.Forms.Button();
            this.btn1ConnectCheck = new System.Windows.Forms.Button();
            this.txt1ConnectionStatus = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtLog2 = new System.Windows.Forms.TextBox();
            this.btnGetIP = new System.Windows.Forms.Button();
            this.lblPluginHint = new System.Windows.Forms.Label();
            this.txtCountDown = new System.Windows.Forms.TextBox();
            this.bgWorkerConnection = new System.ComponentModel.BackgroundWorker();
            this.bgWorkerQFIL = new System.ComponentModel.BackgroundWorker();
            this.bgWorkerEdl = new System.ComponentModel.BackgroundWorker();
            this.lblDHCP = new System.Windows.Forms.Label();
            this.tabControlMenu.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMenu
            // 
            this.tabControlMenu.Controls.Add(this.tabPage1);
            this.tabControlMenu.Controls.Add(this.tabPage2);
            this.tabControlMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMenu.Location = new System.Drawing.Point(0, 0);
            this.tabControlMenu.Name = "tabControlMenu";
            this.tabControlMenu.SelectedIndex = 0;
            this.tabControlMenu.Size = new System.Drawing.Size(800, 450);
            this.tabControlMenu.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.tabPage1.Controls.Add(this.txt1EdlStatus);
            this.tabPage1.Controls.Add(this.pgBarEdl);
            this.tabPage1.Controls.Add(this.btnEdl);
            this.tabPage1.Controls.Add(this.txtLog);
            this.tabPage1.Controls.Add(this.pgBarQFIL);
            this.tabPage1.Controls.Add(this.pgBarConnection);
            this.tabPage1.Controls.Add(this.txt1QFILStatus);
            this.tabPage1.Controls.Add(this.btnQFIL);
            this.tabPage1.Controls.Add(this.btn1ConnectCheck);
            this.tabPage1.Controls.Add(this.txt1ConnectionStatus);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 422);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Step 1: 環境建立";
            // 
            // txt1EdlStatus
            // 
            this.txt1EdlStatus.Location = new System.Drawing.Point(209, 81);
            this.txt1EdlStatus.Name = "txt1EdlStatus";
            this.txt1EdlStatus.ReadOnly = true;
            this.txt1EdlStatus.Size = new System.Drawing.Size(68, 23);
            this.txt1EdlStatus.TabIndex = 10;
            // 
            // pgBarEdl
            // 
            this.pgBarEdl.Location = new System.Drawing.Point(301, 81);
            this.pgBarEdl.Name = "pgBarEdl";
            this.pgBarEdl.Size = new System.Drawing.Size(411, 23);
            this.pgBarEdl.Step = 1;
            this.pgBarEdl.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgBarEdl.TabIndex = 9;
            // 
            // btnEdl
            // 
            this.btnEdl.BackColor = System.Drawing.Color.MistyRose;
            this.btnEdl.Enabled = false;
            this.btnEdl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEdl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEdl.Location = new System.Drawing.Point(68, 81);
            this.btnEdl.Name = "btnEdl";
            this.btnEdl.Size = new System.Drawing.Size(117, 23);
            this.btnEdl.TabIndex = 8;
            this.btnEdl.Text = "切換燒機模式";
            this.btnEdl.UseVisualStyleBackColor = false;
            this.btnEdl.Click += new System.EventHandler(this.btnEdl_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(68, 167);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(644, 235);
            this.txtLog.TabIndex = 7;
            // 
            // pgBarQFIL
            // 
            this.pgBarQFIL.Location = new System.Drawing.Point(301, 124);
            this.pgBarQFIL.Name = "pgBarQFIL";
            this.pgBarQFIL.Size = new System.Drawing.Size(411, 23);
            this.pgBarQFIL.Step = 1;
            this.pgBarQFIL.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgBarQFIL.TabIndex = 6;
            // 
            // pgBarConnection
            // 
            this.pgBarConnection.Location = new System.Drawing.Point(301, 42);
            this.pgBarConnection.Name = "pgBarConnection";
            this.pgBarConnection.Size = new System.Drawing.Size(411, 23);
            this.pgBarConnection.Step = 1;
            this.pgBarConnection.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgBarConnection.TabIndex = 5;
            // 
            // txt1QFILStatus
            // 
            this.txt1QFILStatus.Location = new System.Drawing.Point(209, 124);
            this.txt1QFILStatus.Name = "txt1QFILStatus";
            this.txt1QFILStatus.ReadOnly = true;
            this.txt1QFILStatus.Size = new System.Drawing.Size(68, 23);
            this.txt1QFILStatus.TabIndex = 3;
            // 
            // btnQFIL
            // 
            this.btnQFIL.BackColor = System.Drawing.Color.MistyRose;
            this.btnQFIL.Enabled = false;
            this.btnQFIL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnQFIL.Location = new System.Drawing.Point(68, 124);
            this.btnQFIL.Name = "btnQFIL";
            this.btnQFIL.Size = new System.Drawing.Size(117, 23);
            this.btnQFIL.TabIndex = 2;
            this.btnQFIL.Text = "執行燒機";
            this.btnQFIL.UseCompatibleTextRendering = true;
            this.btnQFIL.UseVisualStyleBackColor = false;
            this.btnQFIL.Click += new System.EventHandler(this.btnQFIL_Click);
            // 
            // btn1ConnectCheck
            // 
            this.btn1ConnectCheck.BackColor = System.Drawing.Color.MistyRose;
            this.btn1ConnectCheck.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn1ConnectCheck.Location = new System.Drawing.Point(68, 42);
            this.btn1ConnectCheck.Name = "btn1ConnectCheck";
            this.btn1ConnectCheck.Size = new System.Drawing.Size(117, 23);
            this.btn1ConnectCheck.TabIndex = 1;
            this.btn1ConnectCheck.Text = "攝影機連接確認";
            this.btn1ConnectCheck.UseVisualStyleBackColor = false;
            this.btn1ConnectCheck.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt1ConnectionStatus
            // 
            this.txt1ConnectionStatus.Location = new System.Drawing.Point(209, 42);
            this.txt1ConnectionStatus.Name = "txt1ConnectionStatus";
            this.txt1ConnectionStatus.ReadOnly = true;
            this.txt1ConnectionStatus.Size = new System.Drawing.Size(68, 23);
            this.txt1ConnectionStatus.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Moccasin;
            this.tabPage2.Controls.Add(this.lblDHCP);
            this.tabPage2.Controls.Add(this.txtLog2);
            this.tabPage2.Controls.Add(this.btnGetIP);
            this.tabPage2.Controls.Add(this.lblPluginHint);
            this.tabPage2.Controls.Add(this.txtCountDown);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 422);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Step 2: 更新金鑰";
            this.tabPage2.Enter += new System.EventHandler(this.tabPage2_Enter);
            this.tabPage2.Leave += new System.EventHandler(this.tabPage2_Leave);
            // 
            // txtLog2
            // 
            this.txtLog2.Location = new System.Drawing.Point(157, 66);
            this.txtLog2.Multiline = true;
            this.txtLog2.Name = "txtLog2";
            this.txtLog2.Size = new System.Drawing.Size(470, 183);
            this.txtLog2.TabIndex = 3;
            // 
            // btnGetIP
            // 
            this.btnGetIP.Location = new System.Drawing.Point(61, 67);
            this.btnGetIP.Name = "btnGetIP";
            this.btnGetIP.Size = new System.Drawing.Size(75, 23);
            this.btnGetIP.TabIndex = 2;
            this.btnGetIP.Text = "取得 IP";
            this.btnGetIP.UseVisualStyleBackColor = true;
            this.btnGetIP.Click += new System.EventHandler(this.btnGetIP_Click);
            // 
            // lblPluginHint
            // 
            this.lblPluginHint.AutoSize = true;
            this.lblPluginHint.Location = new System.Drawing.Point(298, 27);
            this.lblPluginHint.Name = "lblPluginHint";
            this.lblPluginHint.Size = new System.Drawing.Size(329, 15);
            this.lblPluginHint.TabIndex = 1;
            this.lblPluginHint.Text = "計時器停止後, 請插回 USB 線並點選下方按鈕取得 IP 位置";
            // 
            // txtCountDown
            // 
            this.txtCountDown.Location = new System.Drawing.Point(218, 24);
            this.txtCountDown.Name = "txtCountDown";
            this.txtCountDown.ReadOnly = true;
            this.txtCountDown.Size = new System.Drawing.Size(74, 23);
            this.txtCountDown.TabIndex = 0;
            // 
            // bgWorkerConnection
            // 
            this.bgWorkerConnection.WorkerReportsProgress = true;
            this.bgWorkerConnection.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerConnection_DoWork);
            this.bgWorkerConnection.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerConnection_ProgressChanged);
            this.bgWorkerConnection.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerConnection_RunWorkerCompleted);
            // 
            // bgWorkerQFIL
            // 
            this.bgWorkerQFIL.WorkerReportsProgress = true;
            this.bgWorkerQFIL.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerQFIL_DoWork);
            this.bgWorkerQFIL.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerQFIL_ProgressChanged);
            this.bgWorkerQFIL.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerQFIL_RunWorkerCompleted);
            // 
            // bgWorkerEdl
            // 
            this.bgWorkerEdl.WorkerReportsProgress = true;
            this.bgWorkerEdl.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerEdl_DoWork);
            this.bgWorkerEdl.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerEdl_ProgressChanged);
            this.bgWorkerEdl.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerEdl_RunWorkerCompleted);
            // 
            // lblDHCP
            // 
            this.lblDHCP.AutoSize = true;
            this.lblDHCP.Location = new System.Drawing.Point(61, 27);
            this.lblDHCP.Name = "lblDHCP";
            this.lblDHCP.Size = new System.Drawing.Size(151, 15);
            this.lblDHCP.TabIndex = 4;
            this.lblDHCP.Text = "請稍後, DHCP 分配 IP 中...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControlMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OSSA tool";
            this.tabControlMenu.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMenu;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txt1ConnectionStatus;
        private System.Windows.Forms.Button btn1ConnectCheck;
        private System.Windows.Forms.Button btnQFIL;
        private System.Windows.Forms.TextBox txt1QFILStatus;
        private System.ComponentModel.BackgroundWorker bgWorkerConnection;
        private System.ComponentModel.BackgroundWorker bgWorkerQFIL;
        private System.Windows.Forms.ProgressBar pgBarQFIL;
        private System.Windows.Forms.ProgressBar pgBarConnection;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txt1EdlStatus;
        private System.Windows.Forms.ProgressBar pgBarEdl;
        private System.Windows.Forms.Button btnEdl;
        private System.ComponentModel.BackgroundWorker bgWorkerEdl;
        private System.Windows.Forms.TextBox txtCountDown;
        private System.Windows.Forms.Label lblPluginHint;
        private System.Windows.Forms.Button btnGetIP;
        private System.Windows.Forms.TextBox txtLog2;
        private System.Windows.Forms.Label lblDHCP;
    }
}

