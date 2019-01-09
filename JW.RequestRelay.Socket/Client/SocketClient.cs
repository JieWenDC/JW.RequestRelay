using JW.RequestRelay.Util.Json;
using JW.RequestRelay.Util.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JW.RequestRelay.Socket.Client
{
    public class SocketClient
    {
        /// <summary>
        /// 接收消息默认缓冲区大小
        /// </summary>
        private const int Receive_Buffer_Size = 1024;

        [JsonIgnore]
        public System.Net.Sockets.Socket Socket { get; }

        /// <summary>
        /// 是否已释放
        /// </summary>
        public bool IsDisposable { get; set; }

        /// <summary>
        /// 服务端点信息
        /// </summary>
        [JsonIgnore]
        protected IPEndPoint IPE { get; }

        /// <summary>
        /// 本机地址
        /// </summary>
        public string LocalAddress { get; private set; }

        /// <summary>
        /// 接受到消息时会触发该方法执行回调
        /// </summary>
        [JsonIgnore]
        public Action<string> ReceiveCallback { get; set; }

        /// <summary>
        /// 会话关闭时回调
        /// </summary>
        [JsonIgnore]
        public Action<Exception> CloseCallback { get; set; }

        /// <summary>
        /// 接受数据时发送异常后执行
        /// </summary>
        [JsonIgnore]
        public Action<Exception> ReciveExceptionCallback { get; set; }

        /// <summary>
        /// 发送数据发生异常后执行
        /// </summary>
        [JsonIgnore]
        public Action<Exception, object> SendExceptionCallback { get; set; }

        /// <summary>
        /// 与服务端尝试建立连接回调
        /// 连接成功输入参数未NUll,连接失败输入参数未异常信息
        /// </summary>
        [JsonIgnore]
        public Action<Exception> ConnectCallback { get; set; }

        /// <summary>
        /// 初始一个Socket
        /// </summary>
        /// <param name="localEP">监听地址</param>
        /// <param name="callback">当前接受到新消息时回调</param>
        public SocketClient(IPAddress address, int port)
        {
            this.IsDisposable = false;
            IPE = new IPEndPoint(address, port);
            Socket = new System.Net.Sockets.Socket(IPE.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            try
            {
                Socket.Connect(IPE);
                if (!Socket.Connected)
                {
                    throw new Exception($"{this.LocalAddress}与{IPE.Address.ToString()}建立连接失败,状态为={this.Socket.Connected}");
                }
            }
            catch (Exception ex)
            {
                if (this.ConnectCallback != null)
                {
                    this.ConnectCallback(ex);
                }
                throw ex;
            }
            this.LocalAddress = ((System.Net.IPEndPoint)this.Socket.LocalEndPoint).ToString();
            Log4netHelper.Debug($"{this.LocalAddress}已建立与{IPE.Address.ToString()}的连接");
            if (this.ConnectCallback != null)
            {
                this.ConnectCallback(null);
            }
            TrySetKeepAlive(5, 5);
            Task.Run(() =>
            {
                try
                {
                    Receive();
                }
                catch (Exception ex)
                {
                    Log4netHelper.Fatal("Recive异常", ex);
                }
            });
        }

        /// <summary>
        /// 开始接受消息
        /// </summary>
        private void Receive()
        {
            while (this.Socket.Connected)
            {
                var buffer = Receive(null);
                if (buffer != null && buffer.Length > 0)
                {
                    var message_content = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    while (!message_content.EndsWith(SocketCommand.MessageSeparator))//如果不是一条完整的消息，继续接收
                    {
                        buffer = Receive(buffer);
                        message_content = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    }
                    var message_list = message_content.ToListData(separator: SocketCommand.MessageSeparator);
                    foreach (var message_item in message_list)
                    {
                        var message_item_content = string.Empty;
                        try
                        {
                            message_item_content = message_item.DecryptBase64(encode: Encoding.UTF8);
                            ReceiveCallbackAsync(message_item_content);
                        }
                        catch (Exception ex)
                        {
                            Log4netHelper.Fatal($"接受到消息:{message_item}解密异常", ex);
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///接收消息
        /// </summary>
        /// <returns></returns>
        private byte[] Receive(byte[] buffer)
        {
            int receiveBytes = 0;
            try
            {
                var receive_buffer = new byte[Receive_Buffer_Size];
                receiveBytes = this.Socket.Receive(receive_buffer);//receiveBytes长度小于等于buffer的长度
                byte[] add_buffer = null;//新接收到的数据
                byte[] transfer_buffer = null;//中转变量，存储已经接收到的数据
                while (this.Socket.Available > 0)
                {
                    add_buffer = new byte[Receive_Buffer_Size];
                    var add_receiveBytes = this.Socket.Receive(add_buffer);

                    receiveBytes += add_receiveBytes;
                    //创建新增存储对象
                    transfer_buffer = new byte[receiveBytes];
                    //将历史数据复制到新的存储对象
                    Array.Copy(receive_buffer, 0, transfer_buffer, 0, receiveBytes - add_receiveBytes);
                    //将新增的数据复制打中转存储对象
                    Array.Copy(add_buffer, 0, transfer_buffer, receiveBytes - add_receiveBytes, add_receiveBytes);
                    receive_buffer = transfer_buffer;
                }
                if (receiveBytes <= 0)
                {
                    Log4netHelper.Debug($"接受到服务端{this.ToJson()}{receiveBytes}个字节，自动关闭连接");
                    this.Close(new Exception($"接受到服务端{receiveBytes}个字节,自动关闭连接"));
                }
                if (buffer != null)
                {
                    transfer_buffer = new byte[buffer.Length + receive_buffer.Length];
                    Array.Copy(buffer, 0, transfer_buffer, 0, buffer.Length);
                    Array.Copy(receive_buffer, 0, transfer_buffer, buffer.Length, receive_buffer.Length);
                    return transfer_buffer;
                }
                else
                {
                    if (receiveBytes < receive_buffer.Length)
                    {
                        return receive_buffer.Take(receiveBytes).ToArray();
                    }
                    return receive_buffer;
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Fatal($"客户端{this.ToJson()}接受消息时异常,自动关闭连接", ex);
                this.Close(ex);
                if (this.ReciveExceptionCallback != null)
                {
                    this.ReciveExceptionCallback(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// 异步运行接受消息回调
        /// </summary>
        /// <param name="msg"></param>
        void ReceiveCallbackAsync(string msg)
        {
            if (ReceiveCallback == null)
            {
                return;
            }
            Task.Run(() =>
            {
                try
                {
                    ReceiveCallback(msg);
                }
                catch (Exception ex)
                {
                    Log4netHelper.Fatal($"客户端接收到消息但执行ReceiveCallback异常:{msg}", ex);
                }
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        public void Send<T>(T obj)
        {
            try
            {
                var msg = JsonHelper.SerializeObject(obj);
                byte[] buffer = Encoding.UTF8.GetBytes($"{msg.EncryptBase64(encode: Encoding.UTF8)}{SocketCommand.MessageSeparator}");
                var send_byte = Socket.Send(buffer, buffer.Length, SocketFlags.None);
                if (send_byte < buffer.Length)
                {
                    throw new Exception($"send_byte={send_byte}<buffer.length={buffer.Length}");
                }
                Log4netHelper.Info($"发送{buffer.Length}字节,实际发送{send_byte}字节{Environment.NewLine}{msg}");
            }
            catch (Exception ex)
            {
                if (this.SendExceptionCallback != null)
                {
                    this.SendExceptionCallback(ex, obj);
                }
                Log4netHelper.Debug($"发送消息异常：{obj.ToJson()}", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="ex">异常信息，如果主动关闭未Null，如果异常引发关闭则是异常信息</param>
        public void Close(Exception ex = null)
        {

            if (!this.IsDisposable)
            {
                lock (Socket)
                {
                    if (!this.IsDisposable)
                    {
                        Log4netHelper.Debug($"正在关闭客户端与服务端的链接:{IPE.Address.ToString()}");
                        Socket.Shutdown(SocketShutdown.Both);
                        Socket.Close();
                        Socket.Dispose();
                        IsDisposable = true;
                        if (CloseCallback != null)
                        {
                            try
                            {
                                CloseCallback(ex);
                            }
                            catch (Exception close_ex)
                            {
                                Log4netHelper.Fatal($"客户端接收到消息但执行DisposeCallback异常", close_ex);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置会话的心跳包
        /// </summary>
        /// <param name="dueTime">延迟的时间量（以毫秒为单位）</param>
        /// <param name="period">时间间隔（以毫秒为单位）</param>
        /// <returns></returns>
        public bool TrySetKeepAlive(int dueTime, int period)
        {
            var outOptionValue = new byte[12];
            var inOptionValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };
            try
            {
                this.Socket.IOControl(IOControlCode.KeepAliveValues, inOptionValue, outOptionValue);
                return true;
            }
            catch (NotSupportedException ex)
            {
                this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                Log4netHelper.Fatal("设置心跳包异常", ex);
                return true;
            }
            catch (NotImplementedException ex)
            {
                this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                Log4netHelper.Fatal("设置心跳包异常", ex);
                return true;
            }
            catch (Exception ex)
            {
                Log4netHelper.Fatal("设置心跳包异常", ex);
                return false;
            }
        }
    }
}
