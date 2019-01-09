using JW.RequestRelay.Business;
using JW.RequestRelay.Models.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JW.RequestRelay.Client
{
    public partial class Manage : Form
    {
        public ClientBusiness ClientBusiness { get; set; }
        public Manage()
        {
            InitializeComponent();
            ClientBusiness = new ClientBusiness();
            dg_data.DataSource = ClientBusiness.GetList();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            ClientBusiness.StartAll();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            dg_data.ClickAsync(() =>
            {
                dg_data.Invoke(() =>
                {
                    dg_data.DataSource = ClientBusiness.GetList();
                });
            });
        }

        private void Manage_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientBusiness.CloseAll();
        }

        Timer timer = new Timer();
        private void btn_timedRefresh_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
                btn_timedRefresh.Text = "定时刷新";
            }
            else
            {
                timer.Interval = 1000 * 3;
                timer.Tick += btn_refresh_Click;
                timer.Start();
                btn_timedRefresh.Text = "停止刷新";
            }
        }

        private void dg_data_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var list = dg_data.DataSource as List<ClientModel>;
            if (list.ExistsData())
            {
                var row = list[e.RowIndex];
                if (row != null)
                {

                    var child_form = this.ParentForm.MdiChildren;
                    foreach (var form in child_form)
                    {
                        if (form is Detail)
                        {
                            var _detail = form as Detail;
                            if (_detail.Id == row.Id)
                            {
                                _detail.WindowState = FormWindowState.Maximized;
                                return;
                            }
                        }
                    }
                    var detail = new Detail(row.Id);
                    detail.MdiParent = this.MdiParent;
                    detail.Show();
                }
            }
        }
    }
}
