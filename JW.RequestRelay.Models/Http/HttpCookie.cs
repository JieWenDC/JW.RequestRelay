using System;

namespace JW.RequestRelay.Models.Http
{
    public class HttpCookie
    {
        /// <summary>
        /// 获取或设置 cookie 的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置要与当前 cookie 传输的虚拟路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///   获取或设置一个值，该值指示是否传输，即通过 HTTPS 仅使用安全套接字层 (SSL)-的 cookie。
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// 获取或设置一个值，指定 cookie 是由客户端脚本访问。
        /// </summary>
        public bool HttpOnly { get; set; }

        /// <summary>
        /// 获取或设置要将与 cookie 相关联的域。
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 获取或设置的过期日期和时间的 cookie。
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// 获取或设置一个单独的 cookie 值。
        /// </summary>
        public string Value { get; set; }
    }
}
