using JW.RequestRelay.Business;
using JW.RequestRelay.Models;
using JW.RequestRelay.Models.Client;
using JW.RequestRelay.Socket.Client;
using JW.RequestRelay.Util.Json;
using JW.RequestRelay.Util.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JW.RequestRelay.Client
{
    public partial class Detail : Form
    {
        public string Id { get; set; }
        private ClientModel detail { get; set; }

        private ClientBusiness ClientBusiness { get; set; }

        public Detail(string id)
        {
            InitializeComponent();
            ClientBusiness = new ClientBusiness();
            this.Id = id;
            DataBinBaseInfo();

            #region 初始化 实时记录

            this.dgv_record.ColumnCount = 4;
            this.dgv_record.Columns[0].HeaderText = "标识";
            this.dgv_record.Columns[1].HeaderText = "处理时间";
            this.dgv_record.Columns[2].HeaderText = "类型";
            this.dgv_record.Columns[3].HeaderText = "内容";
            this.dgv_record.Columns[0].Width = 130;
            this.dgv_record.Columns[1].Width = 150;
            this.dgv_record.Columns[2].Width = 150;
            this.dgv_record.Columns[3].Width = this.dgv_record.Width - 175 - 150 - 150;
            this.dgv_record.Rows.Clear();

            #endregion

            #region 初始化历史记录

            this.dgv_logs.ColumnCount = 7;
            this.dgv_logs.Columns[0].HeaderText = "会话地址";
            this.dgv_logs.Columns[1].HeaderText = "请求时间";
            this.dgv_logs.Columns[2].HeaderText = "响应时间";
            this.dgv_logs.Columns[3].HeaderText = "历时MS";
            this.dgv_logs.Columns[4].HeaderText = "地址";
            this.dgv_logs.Columns[5].HeaderText = "请求参数";
            this.dgv_logs.Columns[6].HeaderText = "响应结果";

            this.dgv_logs.Columns[0].Width = 130;
            this.dgv_logs.Columns[1].Width = 90;
            this.dgv_logs.Columns[2].Width = 90;
            this.dgv_logs.Columns[3].Width = 70;
            this.dgv_logs.Columns[4].Width = 100;
            this.dgv_logs.Columns[5].Width = (this.dgv_logs.Width - 25 - 130 - 90 * 2 - 70 - 10) / 2;
            this.dgv_logs.Columns[6].Width = (this.dgv_logs.Width - 25 - 130 - 90 * 2 - 70 - 10) / 2;
            this.dgv_logs.Rows.Clear();

            #endregion

            #region 未处理请求
            this.dgv_untreated.ColumnCount = 7;
            this.dgv_untreated.Columns[0].HeaderText = "Id";
            this.dgv_untreated.Columns[1].HeaderText = "阶段";
            this.dgv_untreated.Columns[2].HeaderText = "接收时间";
            this.dgv_untreated.Columns[3].HeaderText = "请求时间";
            this.dgv_untreated.Columns[4].HeaderText = "响应时间";
            this.dgv_untreated.Columns[5].HeaderText = "地址";
            this.dgv_untreated.Columns[6].HeaderText = "接收参数";
            this.dgv_untreated.Columns[0].Width = 150;
            this.dgv_untreated.Columns[1].Width = 150;
            this.dgv_untreated.Columns[2].Width = 150;
            this.dgv_untreated.Columns[3].Width = 150;
            this.dgv_untreated.Columns[4].Width = 150;
            this.dgv_untreated.Columns[5].Width = 150;
            this.dgv_untreated.Columns[6].Width = this.dgv_untreated.Width - 25 - 150 * 6;
            this.dgv_untreated.Rows.Clear();

            #endregion
        }

        /// <summary>
        /// 绑定基本信息
        /// </summary>
        void DataBinBaseInfo()
        {
            detail = ClientBusiness.Get(Id);
            if (detail != null)
            {
                txt_handleUrl.Text = detail.HandleUrl;
                txt_ip.Text = detail.Ip;
                txt_name.Text = detail.Name;
                txt_port.Value = detail.Port;
                txt_summary.Text = detail.Summary;
                txt_maxSession.Value = detail.MaxSession;

                detail.ReceiveCallback -= ReceiveCallback;
                detail.ReceiveCallback += ReceiveCallback;

                detail.CloseCallback -= CloseCallback;
                detail.CloseCallback += CloseCallback;

                detail.ReciveExceptionCallback -= ReciveExceptionCallback;
                detail.ReciveExceptionCallback += ReciveExceptionCallback;

                detail.SendExceptionCallback -= SendExceptionCallback;
                detail.SendExceptionCallback += SendExceptionCallback;

                detail.ConnectCallback -= ConnectCallback;
                detail.ConnectCallback += ConnectCallback;

                ClientBusiness.ProcessedCallback -= ProcessedCallback;
                ClientBusiness.ProcessedCallback += ProcessedCallback;

                ClientBusiness.ProcessCallback -= ProcessCallback;
                ClientBusiness.ProcessCallback += ProcessCallback;

            };
            if (detail.Status == ClientModelStatusEnum.Pause)
            {
                btn_start.Enabled = true;
                btn_stop.Enabled = false;
            }
            else
            {
                btn_stop.Enabled = true;
                btn_start.Enabled = false;
            }
        }

        #region 事件

        /// <summary>
        /// 接受到消息后执行
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="message"></param>
        public void ReceiveCallback(ClientModel clientModel, SocketClient socketClient, string message)
        {
            Record(new RealTimeLog()
            {
                Socket = socketClient,
                Content = message,
                Type = "Receive"
            });
        }

        /// <summary>
        /// 转发请求并响应请求后执行
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="message"></param>
        private void ProcessedCallback(ClientModel clientModel, SocketClient socketClient, Log log)
        {
            Record(new RealTimeLog()
            {
                Content = $"{Encoding.UTF8.GetString(log.Response.Response)}",
                Type = "Processed",
                HandleId = log.Request.Id,
                Socket = socketClient,
            });
            dgv_logs.Invoke(() =>
            {
                if (dgv_logs.Rows.Count >= 1000)
                {
                    dgv_logs.Rows.RemoveAt(dgv_logs.Rows.Count - 1);
                }
                dgv_logs.Rows.Insert(0, new String[] { log.LocalAddress, log.RequestDateTime.ToString("HH:mm:ss.fff"), log.ResponseDateTime.ToString("HH:mm:ss.fff"), log.UseTime.ToString("0"), log.Request.UrlPathAndQuery, Encoding.UTF8.GetString(log.Request.InputStream), Encoding.UTF8.GetString(log.Response.Response) });
            });
        }

        /// <summary>
        /// 处理过程回调
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="message"></param>
        private void ProcessCallback(ClientModel clientModel, SocketClient socketClient, Log log)
        {
            dgv_untreated.Invoke(() =>
            {

                for (int i = 0; i < this.dgv_untreated.Rows.Count; i++)
                {
                    if ((string)dgv_untreated.Rows[i].Cells[0].Value == log.Id)
                    {
                        if (log.Stage == "回复完毕")
                        {
                            dgv_untreated.Rows.RemoveAt(i);
                            return;
                        }
                        dgv_untreated.Rows[i].Cells[1].Value = log.Stage;
                        dgv_untreated.Rows[i].Cells[3].Value = log.StartHttpRelayTime.ToString("HH:mm:ss.fff");
                        dgv_untreated.Rows[i].Cells[4].Value = log.EndHttpRelayTime.ToString("HH:mm:ss.fff");
                        dgv_untreated.Rows[i].Cells[5].Value = log.Request.UrlPathAndQuery;
                        dgv_untreated.Rows[i].Cells[6].Value = log.Request == null ? log.ReceiveMessage : Encoding.UTF8.GetString(log.Request.InputStream);
                        return;
                    }
                }
                if (log.Stage != "回复完毕")
                {
                    dgv_untreated.Rows.Insert(0, new String[] { log.Id, log.Stage, log.RequestDateTime.ToString("HH:mm:ss.fff"), log.StartHttpRelayTime.ToString("HH:mm:ss.fff"), log.EndHttpRelayTime.ToString("HH:mm:ss.fff"), log.Request == null ? "" : log.Request.UrlPathAndQuery, log.Request == null ? log.ReceiveMessage : Encoding.UTF8.GetString(log.Request.InputStream) });
                }
            });
        }

        /// <summary>
        /// 客户端关闭后执行
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="exception"></param>
        private void CloseCallback(ClientModel clientModel, SocketClient socketClient, Exception exception)
        {
            Record(new RealTimeLog()
            {
                Socket = socketClient,
                Content = $"客户端已关闭连接：{(exception == null ? "正常断开" : exception.Message)}",
                Type = "Close"
            });
        }

        /// <summary>
        /// 接收消息异常后执行
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="exception"></param>
        private void ReciveExceptionCallback(ClientModel clientModel, SocketClient socketClient, Exception exception)
        {
            Record(new RealTimeLog()
            {
                Content = $"接受消息异常:{exception.Message}",
                Type = "ReciveException",
                Socket = socketClient,
            });
        }

        /// <summary>
        /// 发送消息异常后执行
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="exception"></param>
        private void SendExceptionCallback(ClientModel clientModel, SocketClient socketClient, Exception exception, string message)
        {
            Record(new RealTimeLog()
            {
                Content = $"发送消息异常:{exception.Message}",
                Socket = socketClient,
                Type = "SendException"
            });
        }

        /// <summary>
        /// 尝试建立连接
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="socketClient"></param>
        /// <param name="exception"></param>
        private void ConnectCallback(ClientModel clientModel, SocketClient socketClient, Exception exception)
        {
            Record(new RealTimeLog()
            {
                Content = exception == null ? "建立连接成功" : exception.Message,
                Socket = socketClient,
                Type = "Connect"
            });
        }

        private void Record(RealTimeLog log)
        {
            dgv_record.Invoke(() =>
            {
                if (dgv_record.Rows.Count >= 100)
                {
                    dgv_record.Rows.RemoveAt(dgv_record.Rows.Count - 1);
                }
                dgv_record.Rows.Insert(0, new String[] { log.LocalAddress, log.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), log.Type, log.Content });
            });
        }

        #endregion

        private void btn_update_Click(object sender, EventArgs e)
        {
            var client = new ClientModel()
            {
                Ip = txt_ip.Text,
                Id = this.Id,
                Name = txt_name.Text,
                Port = txt_port.Value.ToInt(),
                Summary = txt_summary.Text,
                HandleUrl = txt_handleUrl.Text,
                MaxSession = txt_maxSession.Value.ToInt(),
            };
            btn_update.ClickAsync(() =>
            {
                ClientBusiness.Update(client);
            });
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;
            btn_stop.Enabled = false;
            Task.Run(() =>
            {
                Policy.Handle<Exception>().WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), (policy_ex, timer) =>
                {

                }).Execute(() =>
                {
                    ClientBusiness.Start(Id);
                });
                btn_stop.Invoke(() =>
                {
                    btn_stop.Enabled = true;
                });
            });
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            ClientBusiness.Close(this.Id);
            btn_stop.Enabled = false;
            btn_start.Enabled = true;
        }

        /// <summary>
        /// 窗体关闭后释放所有资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Detail_FormClosed(object sender, FormClosedEventArgs e)
        {
            var detail = ClientBusiness.Get(Id);
            if (detail != null)
            {
                detail.ReceiveCallback -= ReceiveCallback;
                ClientBusiness.ProcessedCallback -= ProcessedCallback;
                detail.CloseCallback -= CloseCallback;
                detail.ReciveExceptionCallback -= ReciveExceptionCallback;
                detail.SendExceptionCallback -= SendExceptionCallback;
                detail.ConnectCallback -= ConnectCallback;
            };
        }
    }

}
