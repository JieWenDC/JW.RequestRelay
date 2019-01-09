using JW.RequestRelay.Socket.Server;
using System;
using System.Windows.Forms;

namespace JW.RequestRelay.Tools
{
    public partial class SendMsg : Form
    {
        SocketListener server = null;
        public SendMsg(SocketListener server)
        {
            InitializeComponent();
            this.server = server;
            DataBin();
        }

        public void DataBin()
        {
            cb_clients.Items.Add("--全部--");
            server.SESSIONS.ForEach(session =>
            {
                cb_clients.Items.Add(((System.Net.IPEndPoint)session.RemoteEndPoint).Address.ToString());
            });
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            if (cb_clients.SelectedItem.ToString() == "--全部--")
            {
                server.SendAll(txt_msg.Text);
            }
            server.SESSIONS.ForEach(session =>
            {
                if (((System.Net.IPEndPoint)session.RemoteEndPoint).Address.ToString() == cb_clients.SelectedItem.ToString())
                {
                    session.Send(txt_msg.Text);
                }
            });
        }

        private void SendMsg_Resize(object sender, EventArgs e)
        {

        }
    }
}
