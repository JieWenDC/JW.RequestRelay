using JW.RequestRelay.Util.Json;
using JW.RequestRelay.Util.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JW.RequestRelay.Socket.Server
{
    /// <summary>
    /// 客户端会话信息
    /// </summary>
    public class SocketSession
    {
        public SocketSession(System.Net.Sockets.Socket socket)
        {
            this.IsDisposable = false;
            this.Socket = socket;
        }

        /// <summary>
        /// 是否已释放
        /// </summary>
        public bool IsDisposable { get; set; }

        /// <summary>
        /// 会话Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 远端地址
        /// </summary>
        public EndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// 本机终结点
        /// </summary>
        public EndPoint LocalEndPoint { get; set; }

        /// <summary>
        /// 接受到消息时会触发该方法执行回调
        /// </summary>
        [JsonIgnore]
        public Action<SocketSession, string> ReceiveCallback { get; set; }

        /// <summary>
        /// 会话关闭时回调
        /// </summary>
        [JsonIgnore]
        public Action<SocketSession> DisposeCallback { get; set; }

        /// <summary>
        /// Socket对象
        /// </summary>
        [JsonIgnore]
        public System.Net.Sockets.Socket Socket { get; }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            Task.Run(() =>
            {
                try
                {
                    Receive();
                }
                catch (Exception ex)
                {
                    Log4netHelper.Fatal($"会话在接受消息是发生异常{Environment.NewLine}会话={JsonHelper.SerializeObject(this)}", ex);
                }
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Send<T>(T obj)
        {
            var msg = JsonHelper.SerializeObject(obj);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
            this.Socket.Send(buffer, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// 销毁会话
        /// </summary>
        public void Close()
        {
            if (!this.IsDisposable)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket.Dispose();
                this.IsDisposable = true;
                if (DisposeCallback != null)
                {
                    DisposeCallback(this);
                }
            }
        }

        /// <summary>
        /// 开始接受消息
        /// </summary>
        /// <param name="ar"></param>
        private void Receive()
        {
            while (this.Socket.Connected)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                var receiveBytes = this.Socket.Receive(buffer);
                if (receiveBytes <= 0)
                {
                    Log4netHelper.Debug($"服务端接受到{receiveBytes}个字节，客户端已断开连接");
                    this.Close();
                }
                else
                {
                    var msg = Encoding.UTF8.GetString(buffer, 0, receiveBytes);
                    Log4netHelper.Debug($"服务端接受到来自{this.RemoteEndPoint}的{receiveBytes}个字节消息：{msg}");
                    ReceiveCallbackAsync(this, msg);
                }
            }
        }

        /// <summary>
        /// 异步运行接受消息回调
        /// </summary>
        /// <param name="msg"></param>
        void ReceiveCallbackAsync(SocketSession session, string msg)
        {
            if (ReceiveCallback == null)
            {
                return;
            }
            Task.Run(() =>
            {
                try
                {
                    ReceiveCallback(session, msg);
                }
                catch (Exception ex)
                {
                    Log4netHelper.Fatal($"服务端接收到消息但执行ReceiveCallback异常:{msg}", ex);
                }
            });
        }
    }
}
