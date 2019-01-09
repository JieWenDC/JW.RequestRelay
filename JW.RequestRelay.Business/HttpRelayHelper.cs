using JW.RequestRelay.Models.Http;
using JW.RequestRelay.Util.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JW.RequestRelay.Business
{
    public class HttpRelayHelper
    {
        public static async Task<ResponseModel> HttpRelayAsync(string url, RequestModel param)
        {
            var ret = new ResponseModel()
            {
                Id = param.Id,
            };
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = param.HttpMethod;
            foreach (var item in param.Headers)
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
                    Log4netHelper.Fatal($"设置头信息{item.Key}={item.Value}异常", ex);
                }
            }
            if (param.InputStream.Length > 0)
            {
                byte[] param_form_bytes = param.InputStream;
                request.ContentLength = param_form_bytes.Length;
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    try
                    {
                        requestStream.Write(param_form_bytes, 0, param_form_bytes.Length);
                        requestStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Log4netHelper.Fatal($"写入请求流异常", ex);
                        throw;
                    }
                }
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            finally
            {
                if (response != null)
                {
                    ret.StatusCode = response.StatusCode;
                    if (response.ContentLength > 0)
                    {
                        ret.Response = new byte[response.ContentLength];
                        var responseStream = response.GetResponseStream();
                        var conetntLength = (int)response.ContentLength;
                        var read_byte = await responseStream.ReadAsync(ret.Response, 0, conetntLength);
                        while (read_byte != response.ContentLength)
                        {
                            read_byte += await responseStream.ReadAsync(ret.Response, read_byte, conetntLength - read_byte);
                        }
                    }
                    else
                    {
                        ret.Response = new byte[0];
                    }
                    ret.Headers = response.Headers.ToDictionary();
                    if (response.Cookies != null && response.Cookies.Count > 0)
                    {
                        ret.HttpCookies = new List<Models.Http.HttpCookie>(response.Cookies.Count);
                        foreach (Cookie cookie in response.Cookies)
                        {
                            ret.HttpCookies.Add(new Models.Http.HttpCookie()
                            {
                                Domain = cookie.Domain,
                                Expires = cookie.Expires,
                                HttpOnly = cookie.HttpOnly,
                                Name = cookie.Name,
                                Path = cookie.Path,
                                Secure = cookie.Secure,
                                Value = cookie.Value,
                            });
                        }
                    }
                }
            }
            return ret;

        }

        /// <summary>
        /// 对参数进行升序排序并拼接字符串p1=?&p2=
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GenderParamString(NameValueCollection param)
        {
            var paramStr = string.Empty;
            if (param != null && param.Count > 0)
            {
                var data = new StringBuilder();
                foreach (string key in param.Keys)
                {
                    data.AppendFormat("&{0}={1}", key, param[key]);
                }
                paramStr = data.ToString();
                paramStr = paramStr.Remove(0, 1);
            }
            return paramStr;
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
                foreach (var item in param)
                {
                    data.AppendFormat("&{0}={1}", item.Key, item.Value.UrlEncode());
                }
                paramStr = data.ToString();
                paramStr = paramStr.Remove(0, 1);
            }
            return paramStr;
        }
    }
}
