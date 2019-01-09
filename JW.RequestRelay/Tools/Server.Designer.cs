namespace JW.RequestRelay.Tools
{
    partial class Server
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
            this.btn_serverSendMsg = new System.Windows.Forms.Button();
            this.btn_closeServerClient = new System.Windows.Forms.Button();
            this.txt_serverLogs = new System.Windows.Forms.TextBox();
            this.btn_startServer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_backlog = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.txt_port = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dg_clients = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_backlog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_port)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dg_clients)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_serverSendMsg
            // 
            this.btn_serverSendMsg.Location = new System.Drawing.Point(630, 83);
            this.btn_serverSendMsg.Name = "btn_serverSendMsg";
            this.btn_serverSendMsg.Size = new System.Drawing.Size(75, 23);
            this.btn_serverSendMsg.TabIndex = 17;
            this.btn_serverSendMsg.Text = "发送信息";
            this.btn_serverSendMsg.UseVisualStyleBackColor = true;
            this.btn_serverSendMsg.Click += new System.EventHandler(this.btn_serverSendMsg_Click);
            // 
            // btn_closeServerClient
            // 
            this.btn_closeServerClient.Location = new System.Drawing.Point(630, 53);
            this.btn_closeServerClient.Name = "btn_closeServerClient";
            this.btn_closeServerClient.Size = new System.Drawing.Size(75, 23);
            this.btn_closeServerClient.TabIndex = 16;
            this.btn_closeServerClient.Text = "断开连接";
            this.btn_closeServerClient.UseVisualStyleBackColor = true;
            this.btn_closeServerClient.Click += new System.EventHandler(this.btn_closeServerClient_Click);
            // 
            // txt_serverLogs
            // 
            this.txt_serverLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_serverLogs.Location = new System.Drawing.Point(12, 112);
            this.txt_serverLogs.Multiline = true;
            this.txt_serverLogs.Name = "txt_serverLogs";
            this.txt_serverLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_serverLogs.Size = new System.Drawing.Size(886, 217);
            this.txt_serverLogs.TabIndex = 15;
            this.txt_serverLogs.WordWrap = false;
            // 
            // btn_startServer
            // 
            this.btn_startServer.Location = new System.Drawing.Point(630, 24);
            this.btn_startServer.Name = "btn_startServer";
            this.btn_startServer.Size = new System.Drawing.Size(75, 23);
            this.btn_startServer.TabIndex = 14;
            this.btn_startServer.Text = "启动服务端";
            this.btn_startServer.UseVisualStyleBackColor = true;
            this.btn_startServer.Click += new System.EventHandler(this.btn_startServer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txt_backlog);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txt_address);
            this.groupBox1.Controls.Add(this.txt_port);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(588, 94);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置";
            // 
            // txt_backlog
            // 
            this.txt_backlog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_backlog.Location = new System.Drawing.Point(358, 58);
            this.txt_backlog.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.txt_backlog.Name = "txt_backlog";
            this.txt_backlog.Size = new System.Drawing.Size(120, 21);
            this.txt_backlog.TabIndex = 7;
            this.txt_backlog.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(287, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "最大连接：";
            // 
            // txt_address
            // 
            this.txt_address.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_address.Location = new System.Drawing.Point(82, 20);
            this.txt_address.Name = "txt_address";
            this.txt_address.Size = new System.Drawing.Size(396, 21);
            this.txt_address.TabIndex = 1;
            // 
            // txt_port
            // 
            this.txt_port.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_port.Location = new System.Drawing.Point(82, 58);
            this.txt_port.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(120, 21);
            this.txt_port.TabIndex = 3;
            this.txt_port.Value = new decimal(new int[] {
            7112,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "地址：";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口：";
            // 
            // dg_clients
            // 
            this.dg_clients.AllowUserToAddRows = false;
            this.dg_clients.AllowUserToDeleteRows = false;
            this.dg_clients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dg_clients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_clients.Location = new System.Drawing.Point(12, 335);
            this.dg_clients.Name = "dg_clients";
            this.dg_clients.ReadOnly = true;
            this.dg_clients.RowTemplate.Height = 23;
            this.dg_clients.Size = new System.Drawing.Size(886, 213);
            this.dg_clients.TabIndex = 18;
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 560);
            this.Controls.Add(this.dg_clients);
            this.Controls.Add(this.btn_serverSendMsg);
            this.Controls.Add(this.btn_closeServerClient);
            this.Controls.Add(this.txt_serverLogs);
            this.Controls.Add(this.btn_startServer);
            this.Controls.Add(this.groupBox1);
            this.Name = "Server";
            this.Text = "服务端";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Server_FormClosed);
            this.Resize += new System.EventHandler(this.Server_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_backlog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_port)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dg_clients)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_serverSendMsg;
        private System.Windows.Forms.Button btn_closeServerClient;
        private System.Windows.Forms.TextBox txt_serverLogs;
        private System.Windows.Forms.Button btn_startServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.NumericUpDown txt_port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dg_clients;
        private System.Windows.Forms.NumericUpDown txt_backlog;
        private System.Windows.Forms.Label label4;
    }
}