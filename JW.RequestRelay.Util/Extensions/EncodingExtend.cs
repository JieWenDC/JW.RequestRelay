using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace System
{
    /// <summary>
    /// 编解码扩展
    /// </summary>
    public static class EncodingExtend
    {
        /// <summary>
        /// 特殊字符 转义 编码               入库
        /// </summary>
        /// <param name="_this">原字符串</param>
        /// <returns>返回字符串</returns>
        public static string EscapeChars(this string _this)
        {
            if (string.IsNullOrEmpty(_this))
            {
                return string.Empty;
            }
            else
            {
                return HttpUtility.HtmlEncode(_this);
            }
        }

        /// <summary>
        /// 特殊字符 反转义     解码                   出库
        /// </summary>
        /// <param name="_this">原字符串</param>
        /// <returns>返回字符串</returns>
        public static string UnescapeChars(this string _this)
        {
            if (string.IsNullOrEmpty(_this))
            {
                return string.Empty;
            }
            else
            {
                return HttpUtility.HtmlDecode(_this);
            }
        }

        private const string KEY = "%.L4@9~j$#)U-m=+";

        /// <summary>
        /// 字符串按MD5进行加密
        /// </summary>
        public static string MD5(this string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }
            return byte2String;
        }

        #region DES

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns></returns>
        public static string EncryptDES(this string str)
        {
            return EncryptDES(str, KEY);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string EncryptDES(this string str, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中
            byte[] inputByteArray = System.Text.Encoding.Default.GetBytes(str);

            //建立加密对象的密钥和偏移量
            //使得输入密码必须输入英文文本
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <returns></returns>
        public static string DecryptDES(this string str)
        {
            return DecryptDES(str, KEY);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string DecryptDES(this string str, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[str.Length / 2];
            for (int x = 0; x < str.Length / 2; x++)
            {
                int i = Convert.ToInt32(str.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象
            StringBuilder ret = new StringBuilder();
            return System.Text.Encoding.Default.GetString(ms.ToArray());

        }

        #endregion

        #region BASE64

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string EncryptBase64(this string str, Encoding encode = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (encode == null)
            {
                encode = Encoding.UTF8;
            }
            var bytes = encode.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 揭秘Base64编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string DecryptBase64(this string str, Encoding encode = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (encode == null)
            {
                encode = Encoding.UTF8;
            }
            var bytes = Convert.FromBase64String(str);
            return encode.GetString(bytes);
        }

        #endregion

        #region 加密密码

        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptPassword(this string str)
        {
            return str.MD5();
        }
        #endregion

        #region

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string _this)
        {
            return System.Web.HttpUtility.HtmlEncode(_this);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string _this)
        {
            return System.Web.HttpUtility.HtmlDecode(_this);
        }

        #endregion


        #region

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static string UrlEncode(this string _this)
        {
            return System.Web.HttpUtility.UrlEncode(_this);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static string UrlDecode(this string _this)
        {
            return System.Web.HttpUtility.UrlDecode(_this);
        }

        #endregion
    }
}
