using System;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Extensions to <see cref="MemberInfo"/>.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// 获取成员的单个特性。
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="memberInfo">将被检查特性的成员</param>
        /// <param name="inherit">包括继承的属性</param>
        /// <returns>返回属性对象，如果找到。如果找不到返回null。</returns>
        public static T GetSingleAttributeOrNull<T>(this MemberInfo memberInfo, bool inherit = true) where T : class
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException("memberInfo");
            }
            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);
            if (attrs.Length > 0)
            {
                return (T)attrs[0];
            }

            return default(T);
        }
    }
}
