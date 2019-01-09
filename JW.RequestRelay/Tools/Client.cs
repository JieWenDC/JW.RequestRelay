using JW.RequestRelay.Util.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JW.RequestRelay.Tools
{
    public partial class Client : Form
    {
        private JW.RequestRelay.Socket.Client.SocketClient ClientSocket;
        public Client()
        {
            InitializeComponent();
        }

        private void btn_connectionServer_Click(object sender, EventArgs e)
        {
            btn_connectionServer.Enabled = false;
            Task.Run(() =>
            {
                try
                {
                    ClientRun();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "发送异常");
                    btn_connectionServer.Invoke(() =>
                    {
                        btn_connectionServer.Enabled = true;
                        btn_closeClientConnection.Enabled = false;
                    });
                }
            });
            btn_closeClientConnection.Enabled = true;

        }

        private void btn_closeClientConnection_Click(object sender, EventArgs e)
        {
            btn_closeClientConnection.Enabled = false;
            btn_connectionServer.Enabled = true;
            ClientSocket.Close();
        }

        void ClientRun()
        {
            ClientSocket = new JW.RequestRelay.Socket.Client.SocketClient(IPAddress.Parse(txt_clientAddress.Text), txt_clientPort.Value.ToInt());
            ClientSocket.Start();
            ClientSocket.ReceiveCallback = (msg) =>
            {
                txt_clientLogs.Invoke(() =>
                {
                    txt_clientLogs.AppendText($"接收到服务端发送的小{msg}{Environment.NewLine}");
                });
            };
            ClientSocket.CloseCallback = (ex) =>
            {
                txt_clientLogs.Invoke(() =>
                {
                    txt_clientLogs.AppendText($"连接已关闭{Environment.NewLine}");
                });
            };
            txt_clientLogs.Invoke(() =>
            {
                txt_clientLogs.AppendText($"连接成功{Environment.NewLine}");
            });
            var i = 0;
            while (!ClientSocket.IsDisposable)
            {
                var param = new Dictionary<string, object>() {
                    { "Count", i },
                    { "Time",DateTime.Now.Ticks},
                    { "Content","汉字发送"}
                };
                txt_clientLogs.Invoke(() =>
                {
                    txt_clientLogs.AppendText($"{DateTime.Now.Ticks}客户端发送数据:{JsonHelper.SerializeObject(param)}{Environment.NewLine}");
                });
                ClientSocket.Send(param);
                Thread.Sleep(3000);
                i++;
            }
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ClientSocket != null)
            {
                ClientSocket.Close();
            }
        }

        private void Client_Resize(object sender, EventArgs e)
        {

        }
    }
}
