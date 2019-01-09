using System;

namespace JW.RequestRelay.Util.Xml
{
    public static class XmlExtensions
    {
        /// <summary>
        /// 序列化为XML
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToXml(this object source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            return XmlHelper.ToXml(source);
        }

        /// <summary>
        /// 反序列化为Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T XmlToObject<T>(this string source) where T : class
        {
            if (source.IsNullOrEmpty())
            {
                return default(T);
            }
            return XmlHelper.ToObject<T>(source);
        }
    }
}
