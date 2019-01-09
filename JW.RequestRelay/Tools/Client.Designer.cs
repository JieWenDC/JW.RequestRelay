namespace JW.RequestRelay.Tools
{
    partial class Client
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_closeClientConnection = new System.Windows.Forms.Button();
            this.txt_clientLogs = new System.Windows.Forms.TextBox();
            this.btn_connectionServer = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_clientAddress = new System.Windows.Forms.TextBox();
            this.txt_clientPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_clientPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_closeClientConnection
            // 
            this.btn_closeClientConnection.Location = new System.Drawing.Point(624, 83);
            this.btn_closeClientConnection.Name = "btn_closeClientConnection";
            this.btn_closeClientConnection.Size = new System.Drawing.Size(75, 23);
            this.btn_closeClientConnection.TabIndex = 13;
            this.btn_closeClientConnection.Text = "断开连接";
            this.btn_closeClientConnection.UseVisualStyleBackColor = true;
            this.btn_closeClientConnection.Click += new System.EventHandler(this.btn_closeClientConnection_Click);
            // 
            // txt_clientLogs
            // 
            this.txt_clientLogs.Location = new System.Drawing.Point(12, 112);
            this.txt_clientLogs.Multiline = true;
            this.txt_clientLogs.Name = "txt_clientLogs";
            this.txt_clientLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_clientLogs.Size = new System.Drawing.Size(886, 436);
            this.txt_clientLogs.TabIndex = 12;
            this.txt_clientLogs.WordWrap = false;
            // 
            // btn_connectionServer
            // 
            this.btn_connectionServer.Location = new System.Drawing.Point(624, 24);
            this.btn_connectionServer.Name = "btn_connectionServer";
            this.btn_connectionServer.Size = new System.Drawing.Size(75, 23);
            this.btn_connectionServer.TabIndex = 11;
            this.btn_connectionServer.Text = "连接服务端";
            this.btn_connectionServer.UseVisualStyleBackColor = true;
            this.btn_connectionServer.Click += new System.EventHandler(this.btn_connectionServer_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txt_clientAddress);
            this.groupBox2.Controls.Add(this.txt_clientPort);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(588, 94);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置";
            // 
            // txt_clientAddress
            // 
            this.txt_clientAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_clientAddress.Location = new System.Drawing.Point(136, 20);
            this.txt_clientAddress.Name = "txt_clientAddress";
            this.txt_clientAddress.Size = new System.Drawing.Size(371, 21);
            this.txt_clientAddress.TabIndex = 1;
            this.txt_clientAddress.Text = "192.168.10.101";
            // 
            // txt_clientPort
            // 
            this.txt_clientPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_clientPort.Location = new System.Drawing.Point(136, 58);
            this.txt_clientPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.txt_clientPort.Name = "txt_clientPort";
            this.txt_clientPort.Size = new System.Drawing.Size(120, 21);
            this.txt_clientPort.TabIndex = 3;
            this.txt_clientPort.Value = new decimal(new int[] {
            7112,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "地址：";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "端口：";
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 560);
            this.Controls.Add(this.btn_closeClientConnection);
            this.Controls.Add(this.txt_clientLogs);
            this.Controls.Add(this.btn_connectionServer);
            this.Controls.Add(this.groupBox2);
            this.Name = "Client";
            this.Text = "客户端";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Client_FormClosed);
            this.Resize += new System.EventHandler(this.Client_Resize);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_clientPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_closeClientConnection;
        private System.Windows.Forms.TextBox txt_clientLogs;
        private System.Windows.Forms.Button btn_connectionServer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txt_clientAddress;
        private System.Windows.Forms.NumericUpDown txt_clientPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}