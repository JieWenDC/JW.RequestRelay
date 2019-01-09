using JW.RequestRelay.Util.Json;
using JW.RequestRelay.Util.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JW.RequestRelay.Socket.Server
{
    /// <summary>
    /// 服务端Socket
    /// </summary>
    public class SocketListener
    {
        /// <summary>
        /// 是否已释放
        /// </summary>
        public bool IsDisposable { get; set; }

        protected System.Net.Sockets.Socket SOCKET_SERVER { get; }

        /// <summary>
        /// 与本服务端建立连接的客户端列表
        /// </summary>
        public List<SocketSession> SESSIONS { get; }

        protected IPEndPoint IPE { get; }

        /// <summary>
        /// 表示将用于处理Socket事件数据的事件的方法
        /// </summary>
        /// <param name="msg"></param>
        public delegate void SocketHandler(SocketSession session, string msg);

        /// <summary>
        /// 接收到客户端发送的数据后发生
        /// </summary>
        public event SocketHandler ReceiveEvent;

        /// <summary>
        ///接收到客户端建立连接请求后发生
        /// </summary>
        public event SocketHandler AcceptConnectionEvent;

        /// <summary>
        /// 初始一个服务端Socket
        /// </summary>
        /// <param name="address">本地Socket地址</param>
        /// <param name="port">端口</param>
        public SocketListener(IPAddress address, int port)
        {
            this.IsDisposable = false;
            SESSIONS = new List<SocketSession>();
            IPE = new IPEndPoint(address, port);
            SOCKET_SERVER = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="backlog"> 挂起的连接队列的最大长度。</param>
        public void Start(int backlog = 10)
        {
            SOCKET_SERVER.Bind(IPE);
            SOCKET_SERVER.Listen(backlog);
            SOCKET_SERVER.BeginAccept(new AsyncCallback(BeginAcceptConnection), null);
            Log4netHelper.Debug($"监听{IPE.Address.ToString()}成功");
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (!this.IsDisposable)
            {
                SendAll("服务端请求断开链接");
                Log4netHelper.Debug("正在关闭服务端");
                SOCKET_SERVER.Close();
                SOCKET_SERVER.Dispose();
                this.IsDisposable = true;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        public void SendAll<T>(T obj)
        {
            SESSIONS.ForEach(session =>
            {
                if (!session.IsDisposable)
                {
                    session.Send(obj);
                }
            });
        }

        /// <summary>
        /// 接受连接
        /// </summary>
        /// <param name="ar"></param>
        private void BeginAcceptConnection(IAsyncResult ar)
        {
            if (this.IsDisposable)
            {
                return;
            }
            var socket = SOCKET_SERVER.EndAccept(ar);
            var remoteEndPoint = ((System.Net.IPEndPoint)socket.RemoteEndPoint);
            Log4netHelper.Debug($"{remoteEndPoint.Address.ToString()}连接到了本服务端");
            var session = new SocketSession(socket)
            {
                Id = Guid.NewGuid().ToString(),
                RemoteEndPoint = socket.RemoteEndPoint,
                ReceiveCallback = (obj, msg) =>
                {
                    try
                    {
                        ReceiveEvent(obj, msg);
                    }
                    catch (Exception ex)
                    {
                        Log4netHelper.Fatal($"服务端在执行ReceiveEvent事件时发生异常{Environment.NewLine}消息内容{msg}{Environment.NewLine}会话{JsonHelper.SerializeObject(obj)}", ex);
                    }
                },
                DisposeCallback = (obj) =>
                {
                    SESSIONS.Remove(obj);
                },
            };
            session.Start();
            SESSIONS.Add(session);
            SOCKET_SERVER.BeginAccept(new AsyncCallback(BeginAcceptConnection), null);
            AcceptConnectionEvent(session, null);
        }

        public void Close()
        {
            this.Stop();
        }
    }
}
