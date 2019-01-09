using JW.RequestRelay.Models.Http;
using System;

namespace JW.RequestRelay.Models
{
    public class Log
    {

        /// <summary>
        /// 日志标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 源Id
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        public SourceTypeEnum SourceType { get; set; }

        /// <summary>
        /// 接受请求时间
        /// </summary>
        public DateTime RequestDateTime { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public DateTime ResponseDateTime { get; set; }

        /// <summary>
        /// 开始转发请求时间
        /// </summary>
        public DateTime StartHttpRelayTime { get; set; }

        /// <summary>
        /// 结束转发请求时间
        /// </summary>
        public DateTime EndHttpRelayTime { get; set; }

        /// <summary>
        /// 接受内容
        /// </summary>
        public RequestModel Request { get; set; }

        /// <summary>
        /// 接收消息
        /// </summary>
        public string ReceiveMessage { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        public ResponseModel Response { get; set; }

        /// <summary>
        /// 是否回复成功
        /// </summary>
        public bool? Reply { get; set; }

        /// <summary>
        /// 是否转发成功
        /// </summary>
        public bool? Relay { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 本地地址
        /// </summary>
        public string LocalAddress { get; set; }

        /// <summary>
        /// 用时
        /// </summary>
        public double UseTime
        {
            get
            {
                return (this.ResponseDateTime - this.RequestDateTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// 阶段
        /// </summary>
        public string Stage { get; set; }
    }

    public enum SourceTypeEnum
    {
        /// <summary>
        /// 客户端
        /// </summary>
        Client = 1,

        /// <summary>
        /// 服务端
        /// </summary>
        Server = 2,
    }
}
