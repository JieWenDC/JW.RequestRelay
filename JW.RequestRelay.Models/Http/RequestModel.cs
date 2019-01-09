using System.Collections.Generic;

namespace JW.RequestRelay.Models.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestModel
    {
        /// <summary>
        /// 请求标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 传入的 HTTP 实体主体的内容
        /// </summary>
        public byte[] InputStream { get; set; }

        /// <summary>
        /// 请求头
        /// </ssummary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// 获取客户端使用的 HTTP 数据传输方法(如 GET、POST 或 HEAD)。
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// URL绝对路径
        /// </summary>
        public string UrlPathAndQuery { get; set; }

        /// <summary>
        /// Cookie
        /// </summary>
        public List<HttpCookie> HttpCookies { get; set; }

    }
}
