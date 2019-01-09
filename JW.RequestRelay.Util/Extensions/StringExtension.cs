namespace System
{
    public static class StringExtension
    {
        /// <summary>
        /// 检查字符串为空为null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// 检查字符串是否为空为NULL
        /// </summary>
        /// <param name="source"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string CheckIsNullOrEmpty(this string source, string error)
        {
            source.IsNullOrEmpty().CheckIsTrue(error);
            return source;
        }

    }
}
