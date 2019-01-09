namespace JW.RequestRelay.Client
{
    partial class Manage
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
            this.dg_data = new System.Windows.Forms.DataGridView();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.btn_timedRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg_data)).BeginInit();
            this.SuspendLayout();
            // 
            // dg_data
            // 
            this.dg_data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dg_data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_data.Location = new System.Drawing.Point(12, 41);
            this.dg_data.Name = "dg_data";
            this.dg_data.RowTemplate.Height = 23;
            this.dg_data.Size = new System.Drawing.Size(901, 525);
            this.dg_data.TabIndex = 0;
            this.dg_data.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dg_data_RowHeaderMouseDoubleClick);
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(12, 12);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "启动全部";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_refresh
            // 
            this.btn_refresh.Location = new System.Drawing.Point(110, 12);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(75, 23);
            this.btn_refresh.TabIndex = 2;
            this.btn_refresh.Text = "刷新";
            this.btn_refresh.UseVisualStyleBackColor = true;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // btn_timedRefresh
            // 
            this.btn_timedRefresh.Location = new System.Drawing.Point(218, 12);
            this.btn_timedRefresh.Name = "btn_timedRefresh";
            this.btn_timedRefresh.Size = new System.Drawing.Size(75, 23);
            this.btn_timedRefresh.TabIndex = 3;
            this.btn_timedRefresh.Text = "定时刷新";
            this.btn_timedRefresh.UseVisualStyleBackColor = true;
            this.btn_timedRefresh.Click += new System.EventHandler(this.btn_timedRefresh_Click);
            // 
            // Manage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 566);
            this.Controls.Add(this.btn_timedRefresh);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.dg_data);
            this.Name = "Manage";
            this.Text = "客户端管理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Manage_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dg_data)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dg_data;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Button btn_timedRefresh;
    }
}