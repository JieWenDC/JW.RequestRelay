namespace System
{
    /// <summary>
    /// 检查类扩展
    /// </summary>
    public static class CheckExtend
    {
        /// <summary>
        /// 检查是不是False，如果是False抛出异常
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="msg"></param>
        public static void CheckIsFalse(this bool _this, string msg)
        {
            if (!_this)
            {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>
        /// 检查是不是True,如果是True抛出异常
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="msg"></param>
        public static void CheckIsTrue(this bool _this, string msg)
        {
            if (_this)
            {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="error"></param>
        public static void CheckNull<T>(this T source, string error) where T : class
        {
            if (source == null)
            {
                throw new ArgumentException(error);
            }
        }

        /// <summary>
        /// 检查时间格式是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="error"></param>
        public static void CheckDateNull(this DateTime source, string error)
        {
            if (source == null || source == default(DateTime))
            {
                throw new ArgumentException(error);
            }
        }

        /// <summary>
        /// 检查时间是否为空
        /// </summary>
        /// <param name="source"></param>
        /// <param name="error"></param>
        public static void CheckDateNull(this DateTime? source, string error)
        {
            if (source == null || source == default(DateTime))
            {
                throw new ArgumentException(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="error"></param>
        public static void CheckNull<T>(this T source, string error, params object[] args) where T : class
        {
            if (source == null)
            {
                throw new ArgumentException(string.Format(error, args));
            }
        }
    }
}
