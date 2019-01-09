using JW.RequestRelay.Socket.Client;
using System;

namespace JW.RequestRelay.Models
{
    /// <summary>
    /// 实时记录
    /// </summary>
    public class RealTimeLog
    {
        public RealTimeLog()
        {
            this.CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 使用Socket
        /// </summary>
        public SocketClient Socket { get; set; }

        /// <summary>
        /// 本地地址
        /// </summary>
        public string LocalAddress
        {
            get
            {
                if (this.Socket != null)
                {
                    return this.Socket.LocalAddress;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 请求、响应处理标识
        /// </summary>
        public string HandleId { get; set; }

        /// <summary>
        /// 类型标识
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 处理内容
        /// </summary>
        public string Content { get; set; }
    }
}
