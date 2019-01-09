using JW.RequestRelay.Util.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace JW.RequestRelay.Util.Web
{
    public partial class HttpHelper
    {
        /// <summary>
        /// 获取当前请求的IP地址
        /// </summary>
        /// <returns></returns>
        public static string IP
        {
            get
            {
                return GetIp(System.Web.HttpContext.Current.Request);
            }
        }

        public static string GetIp(HttpRequest Request)
        {
            string ip4address = String.Empty;
            try
            {
                foreach (IPAddress IPA in Dns.GetHostAddresses(Request.UserHostAddress))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        ip4address = IPA.ToString();
                        break;
                    }
                }

                if (ip4address != String.Empty)
                {
                    return ip4address;
                }

                foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        ip4address = IPA.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return ip4address;

        }

        /// <summary>
        /// 获取Request所有参数 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetRequestParam()
        {
            var request = HttpContext.Current.Request;
            var ret = new Dictionary<string, Dictionary<string, string>>();
            if (request.QueryString.AllKeys.Count() > 0)
            {
                var param = new Dictionary<string, string>();
                foreach (var item in request.QueryString.AllKeys)
                {
                    var key = item;
                    if (string.IsNullOrEmpty(key))
                    {
                        key = "NullKey";
                    }
                    if (param.ContainsKey(key))
                    {
                        param[key] = param[key] + " " + request.QueryString[key] ?? string.Empty;
                    }
                    else
                    {
                        param.Add(key, request.QueryString[key] ?? string.Empty);
                    }
                }
                ret.Add("QueryString", param);
            }
            if (request.Form.AllKeys.Count() > 0)
            {
                var param = new Dictionary<string, string>();
                foreach (var item in request.Form.AllKeys)
                {
                    var key = item;
                    if (string.IsNullOrEmpty(key))
                    {
                        key = "NullKey";
                    }
                    if (param.ContainsKey(key))
                    {
                        param[key] = param[key] + " " + request.Form[key] ?? string.Empty;
                    }
                    else
                    {
                        param.Add(key, request.Form[key] ?? string.Empty);
                    }
                }
                ret.Add("Form", param);
            }
            return ret;
        }

        public static string HttpPost(string url, Dictionary<string, string> param, Dictionary<string, string> headers = null, string charset = "UTF-8", int timeout = 15000)
        {
            var data = GenderParamString(param);
            return Policy.Handle<WebException>().WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), (ex, timer, c, context) =>
                    {
                        Log4netHelper.Fatal("HttpPost异常", ex);
                        Log4netHelper.Fatal($"执行失败! 重试次数 {c} timer={timer.ToString()}i={context.ToJson()}");
                    }).Execute(() =>
                    {
                        return HttpPost(url, data, headers, charset, timeout);
                    });
        }

        /// <summary>
        /// 发送POTS 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private static string HttpPost(string url, string data, Dictionary<string, string> headers = null, string charset = "UTF-8", int timeout = 15000)
        {
#if DEBUG
            timeout = 1000 * 60 * 10;
#endif
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = timeout;
            request.AllowAutoRedirect = false;
            SetHeaders(request, headers);
            byte[] paramBytes = Encoding.GetEncoding(charset).GetBytes(data);
            request.ContentLength = paramBytes.Length;
            string responseStr = string.Empty;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(paramBytes, 0, paramBytes.Length);
                requestStream.Close();
                try
                {
                    var response = request.GetResponse();
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    Log4netHelper.Fatal("HttpPost异常", ex);
                    var response = (HttpWebResponse)ex.Response;
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            responseStr = reader.ReadToEnd();
                        }
                    }
                    Log4netHelper.Debug(url, responseStr);
                    throw ex;
                }
                return responseStr;
            }
        }

        public static string HttpGet(string url, Dictionary<string, string> param = null, Dictionary<string, string> headers = null, string charset = "UTF-8", int timeout = 15000)
        {
            if (param.ExistsData())
            {
                var data = GenderParamString(param);
                url = string.Format("{0}?{1}", url, data);
            }
            var policy = Policy.Handle<WebException>().WaitAndRetry(5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                (ex, timer, c, context) =>
                {
                    Log4netHelper.Fatal("HttpGet异常", ex);
                    Log4netHelper.Fatal($"执行失败! 重试次数 {c} timer={timer.ToString()}i={context.ToJson()}");
                });
            return policy.Execute(() =>
            {
                return HttpGet(url, headers, charset, timeout);
            });
        }

        private static string HttpGet(string url, Dictionary<string, string> headers = null, string charset = "UTF-8", int timeout = 15000)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = timeout;
            request.AllowAutoRedirect = false;
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            WebResponse response = null;
            string responseStr = string.Empty;
            response = request.GetResponse();
            if (response != null)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            return responseStr;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <param name="headers"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string UploadFile(string url, Dictionary<string, string> data, string filePath, Dictionary<string, string> headers = null, string charset = "UTF-8")
        {
            var policy = Policy.Handle<WebException>().WaitAndRetry(5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                (ex, timer, c, context) =>
                {
                    Log4netHelper.Fatal("UploadFile异常", ex);
                    Log4netHelper.Fatal($"执行失败! 重试次数 {c} timer={timer.ToString()}i={context.ToJson()}");
                });
            return policy.Execute(() =>
             {
                 // 设置参数
                 HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                 request.AllowAutoRedirect = true;
                 request.Method = "POST";
                 SetHeaders(request, headers);

                 string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                 request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                 byte[] start_BoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                 byte[] end_BoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                 string fileName = FileHelper.GetFileName(filePath, true);
                 //请求头部信息 
                 StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                 byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());
                 //POST请求数据
                 var postData = new List<byte[]>();
                 if (data != null)
                 {
                     foreach (var item in data)
                     {
                         var str = string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, item.Key, item.Value);
                         byte[] postBytes = Encoding.GetEncoding(charset).GetBytes(str);
                         postData.Add(postBytes);
                     }
                 }

                 using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                 {
                     byte[] bArr = new byte[fs.Length];
                     fs.Read(bArr, 0, bArr.Length);
                     request.ContentLength = start_BoundaryBytes.Length + postHeaderBytes.Length + bArr.Length + end_BoundaryBytes.Length;
                     if (postData.ExistsData())
                     {
                         request.ContentLength = request.ContentLength + postData.Sum(row => row.Length);
                     }
                     using (Stream postStream = request.GetRequestStream())
                     {
                         postStream.Write(start_BoundaryBytes, 0, start_BoundaryBytes.Length);
                         postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                         postStream.Write(bArr, 0, bArr.Length);
                         if (postData.ExistsData())
                         {
                             postData.ForEach(item =>
                             {
                                 postStream.Write(item, 0, item.Length);
                             });
                         }
                         postStream.Write(end_BoundaryBytes, 0, end_BoundaryBytes.Length);
                         //发送请求并获取相应回应数据
                         using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                         {
                             //直到request.GetResponse()程序才开始向目标网页发送Post请求
                             using (Stream instream = response.GetResponseStream())
                             {
                                 using (StreamReader sr = new StreamReader(instream, Encoding.UTF8))
                                 {
                                     //返回结果网页（html）代码
                                     string content = sr.ReadToEnd();
                                     return content;
                                 }
                             }
                         }
                     }
                 }
             });
        }

        /// <summary>
        /// 对参数进行升序排序并拼接字符串p1=?&p2=
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GenderParamString(Dictionary<string, string> param)
        {
            var paramStr = string.Empty;
            if (param.ExistsData())
            {
                var data = new StringBuilder();
                var keys = param.Keys.ToList().OrderBy(item => item);
                foreach (var key in keys)
                {
                    data.AppendFormat("&{0}={1}", key, param[key]);
                }
                paramStr = data.ToString();
                paramStr = paramStr.Remove(0, 1);
            }
            return paramStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GenderParamString(string url, Dictionary<string, string> param)
        {
            var paramStr = string.Empty;
            if (param.ExistsData())
            {
                var data = new StringBuilder();
                var keys = param.Keys.ToList().OrderBy(item => item);
                foreach (var key in keys)
                {
                    data.AppendFormat("&{0}={1}", key, param[key]);
                }
                paramStr = data.ToString();
                paramStr = paramStr.Remove(0, 1);
            }
            if (url.IndexOf("?") != -1)
            {
                return string.Format("{0}&{1}", url, paramStr);
            }
            else
            {
                return string.Format("{0}?{1}", url, paramStr);
            }
        }

        /// <summary>
        /// 将URL转换为可在请求客户端使用的URL。
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string ResolveUrl(string relativeUrl, HttpRequest request = null)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                throw new ArgumentException("相对路径不能未空", "relativeUrl");
            }
            if (request == null)
            {
                if (HttpContext.Current != null)
                {
                    request = HttpContext.Current.Request;
                }
                else
                {
                    throw new ArgumentNullException("httpContext/request");
                }
            }
            if (relativeUrl.StartsWith("~"))
            {
                relativeUrl = relativeUrl.TrimStart('~');
                var basePath = request.ApplicationPath;
                if (basePath.EndsWith("/"))
                {
                    basePath = basePath.Substring(0, basePath.Length - 1);
                }
                if (string.IsNullOrEmpty(basePath) || basePath == "/")
                {
                    return relativeUrl;
                }
                else
                {
                    return string.Format("{0}{1}", basePath, relativeUrl);
                }
            }
            return relativeUrl;
        }

        /// <summary>
        /// 获取指定请求的所有头部信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHeaders(HttpRequest request)
        {
            var ret = new Dictionary<string, string>();
            foreach (var key in request.Headers.AllKeys)
            {
                ret.Add(key, request.Headers[key]);
            }
            return ret;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        public static void SetHeaders(HttpWebRequest request, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case "Referer":
                                request.Referer = item.Value;
                                break;
                            case "Connection":
                                break;
                            case "Keep-Alive":
                                break;
                            case "Content-Length":
                                request.ContentLength = item.Value.ToLong();
                                break;
                            case "Content-Type":
                                request.ContentType = item.Value;
                                break;
                            case "Accept":
                                request.Accept = item.Value;
                                break;
                            case "Host":
                                request.Host = item.Value;
                                break;
                            case "User-Agent":
                                request.UserAgent = item.Value;
                                break;
                            default:
                                request.Headers.Add(item.Key, item.Value);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netHelper.Fatal("SetHeaders异常" + item.Key, ex);
                    }
                }
            }

        }
    }
}
