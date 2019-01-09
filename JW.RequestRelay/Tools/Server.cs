using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JW.RequestRelay.Tools
{
    public partial class Server : Form
    {
        private JW.RequestRelay.Socket.Server.SocketListener ServerSocket;

        public Server()
        {
            InitializeComponent();
        }

        private void btn_startServer_Click(object sender, EventArgs e)
        {
            btn_startServer.Enabled = false;
            Task.Run(() =>
            {
                try
                {
                    ServerRun();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "发送异常");
                    btn_startServer.Invoke(() =>
                    {
                        btn_startServer.Enabled = true;
                    });
                }
            });
        }

        private void btn_closeServerClient_Click(object sender, EventArgs e)
        {
            ServerSocket.Close();
        }

        private void btn_serverSendMsg_Click(object sender, EventArgs e)
        {
            var form = new SendMsg(ServerSocket);
            form.Show();
        }

        void ServerRun()
        {
            var ip = IPAddress.Any;
            if (!string.IsNullOrEmpty(txt_address.Text))
            {
                ip = IPAddress.Parse(txt_address.Text);
            }
            ServerSocket = new JW.RequestRelay.Socket.Server.SocketListener(ip, txt_port.Value.ToInt());
            ServerSocket.AcceptConnectionEvent += new Socket.Server.SocketListener.SocketHandler((session, msg) =>
            {
                txt_serverLogs.Invoke(() =>
                {
                    txt_serverLogs.AppendText($"{session.RemoteEndPoint}与本服务端建立了连接{Environment.NewLine}");
                    dg_clients.DataSource = ServerSocket.SESSIONS;
                });
            });
            ServerSocket.ReceiveEvent += new Socket.Server.SocketListener.SocketHandler((session, msg) =>
            {
                txt_serverLogs.Invoke(() =>
                {
                    txt_serverLogs.AppendText($"服务端接受到来自{session.RemoteEndPoint}的消息：{msg}{Environment.NewLine}");
                });
            });

            ServerSocket.Start(backlog: txt_backlog.Value.ToInt());
            txt_serverLogs.Invoke(() =>
            {
                txt_serverLogs.AppendText($"启动成功{Environment.NewLine}");
            });
        }

        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ServerSocket != null)
            {
                ServerSocket.Close();
            }
        }

        private void Server_Resize(object sender, EventArgs e)
        {

        }
    }
}
