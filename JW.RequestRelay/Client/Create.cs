using JW.RequestRelay.Business;
using JW.RequestRelay.Models.Client;
using System;
using System.Windows.Forms;

namespace JW.RequestRelay.Client
{
    public partial class Create : Form
    {
        public Create()
        {
            InitializeComponent();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var client = new ClientModel()
            {
                Ip = txt_ip.Text,
                Id = Guid.NewGuid().ToString(),
                Name = txt_name.Text,
                Port = txt_port.Value.ToInt(),
                Summary = txt_summary.Text,
                HandleUrl=txt_handleUrl.Text,
                MaxSession=txt_maxSession.Value.ToInt(),
            };
            btn_save.ClickAsync(() =>
            {
                new ClientBusiness().Add(client);
            });
        }
    }
}
