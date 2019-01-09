using System.Collections.Generic;
using System.Net;

namespace JW.RequestRelay.Models.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// 请求标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取 HTTP 头集合
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// 返回的文本
        /// </summary>
        public byte[] Response { get; set; }

        /// <summary>
        /// 获取响应的状态
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Cookie
        /// </summary>
        public List<HttpCookie> HttpCookies { get; set; }

    }
}
