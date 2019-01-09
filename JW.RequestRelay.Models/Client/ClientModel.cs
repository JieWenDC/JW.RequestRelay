using JW.RequestRelay.Socket.Client;
using JW.RequestRelay.Util.Logging;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace JW.RequestRelay.Models.Client
{
    /// <summary>
    /// 客户端配置
    /// </summary>
    public class ClientModel
    {
        public ClientModel()
        {
            this.Sessions = new ConcurrentDictionary<string, SocketClient>();
        }

        /// <summary>
        /// 客户端Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户端描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 服务端端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 服务端IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 转发给该地址处理请求
        /// </summary>
        public string HandleUrl { get; set; }

        /// <summary>
        /// 最多保持N个会话
        /// </summary>
        public int MaxSession { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [XmlIgnore]
        public ClientModelStatusEnum Status { get; set; }

        /// <summary>
        /// 会话数
        /// </summary>
        public int SessionCount
        {
            get
            {
                return Sessions.Count();
            }
        }

        /// <summary>
        /// 会话信息
        /// </summary>
        [XmlIgnore]
        public ConcurrentDictionary<string, SocketClient> Sessions { get; set; }

        /// <summary>
        /// 接受消息
        /// </summary>
        public event ReceiveCallbackEventHandler ReceiveCallback;

        /// <summary>
        /// 客户端关闭
        /// </summary>
        public event CloseCallbackEventHandler CloseCallback;

        /// <summary>
        /// 接受数据时发生异常后执行
        /// </summary>
        public event ReciveExceptionCallbackEventHandler ReciveExceptionCallback;

        /// <summary>
        /// 发送数据发生异常后执行
        /// </summary>
        public event SendExceptionCallbackEventHandler SendExceptionCallback;

        /// <summary>
        /// 尝试与服务端建立连接，失败则Exception未异常西南西，成功Exception为NULL
        /// </summary>
        public event ConnectCallbackEventHandler ConnectCallback;

        /// <summary>
        /// 为本客户端创建一个新的会话
        /// </summary>
        /// <returns></returns>
        public SocketClient CreateSession()
        {
            var socketClient = new SocketClient(IPAddress.Parse(this.Ip), this.Port);
            socketClient.ReceiveCallback = (msg) =>
            {
                if (this.ReceiveCallback != null)
                {
                    this.ReceiveCallback(this, socketClient, msg);
                }
            };
            socketClient.CloseCallback = (ex) =>
            {
                if (this.CloseCallback != null)
                {
                    this.CloseCallback(this, socketClient, ex);
                }
                SocketClient output;
                if (!this.Sessions.TryRemove(socketClient.LocalAddress, out output))
                {
                    Log4netHelper.Debug($"会话关闭后，移除会话失败{socketClient.LocalAddress}");
                }
                if (ex != null)
                {
                    //如果是异常引发的关闭，则重新尝试连接
                    Policy.Handle<Exception>().WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), (policy_ex, timer) =>
                    {
                        var msg = $"{socketClient.LocalAddress}异常关闭，正在重新连接,timer={timer.TotalSeconds} ex={policy_ex.Message}";
                        socketClient.ConnectCallback(new Exception(msg));
                    }).Execute(() =>
                    {
                        CreateSession();
                    });
                }
            };
            socketClient.SendExceptionCallback = (ex, message) =>
            {
                if (this.SendExceptionCallback != null)
                {
                    this.SendExceptionCallback(this, socketClient, ex, message.ToString());
                }
            };
            socketClient.ReciveExceptionCallback = (ex) =>
            {
                if (this.ReciveExceptionCallback != null)
                {
                    this.ReciveExceptionCallback(this, socketClient, ex);
                }
            };
            socketClient.ConnectCallback = (ex) =>
            {
                if (this.ConnectCallback != null)
                {
                    this.ConnectCallback(this, socketClient, ex);
                }
            };
            socketClient.Start();
            if (!this.Sessions.TryAdd(socketClient.LocalAddress, socketClient))
            {
                Log4netHelper.Debug($"{socketClient.LocalAddress}与服务端建立连接后插入会话集合失败");
            }
            this.Status = ClientModelStatusEnum.Running;
            return socketClient;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            foreach (var item in this.Sessions)
            {
                item.Value.Close();
            }
            this.Status = ClientModelStatusEnum.Pause;
        }

        /// <summary>
        /// 获取一个可用Socket客户端
        /// </summary>
        /// <returns></returns>
        public SocketClient GetOneFreeOnlineClient()
        {
            return this.Sessions.Values.Where(row => !row.IsDisposable && row.Socket.Connected).FirstOrDefault();
        }
    }

    public enum ClientModelStatusEnum
    {
        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,
    }
}
