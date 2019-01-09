using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 检查集合是否存在数据
        /// </summary>
        public static bool ExistsData<T>(this ICollection<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        /// <summary>
        /// 检查集合是否为空为NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// 检查集合是否未空未NUll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void CheckIsNullOrEmpty<T>(this ICollection<T> source, string msg)
        {
            source.IsNullOrEmpty<T>().CheckIsTrue(msg);
        }

        /// <summary>
        /// 将项添加到集合，如果它不是已经在收集。
        /// </summary>
        /// <param name="source">集合</param>
        /// <param name="item">项</param>
        /// <typeparam name="T">泛型/typeparam>
        /// <returns>如果返回，返回true，如果不返回false</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (source.Contains(item))
            {
                return false;
            }
            source.Add(item);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <param name="_this"></param>
        /// <param name="collection"></param>
        public static void AddRange<Key, Value>(this IDictionary<Key, Value> _this, IDictionary<Key, Value> collection)
        {
            if (collection.ExistsData())
            {
                foreach (var item in collection)
                {
                    _this.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow dr, string columnName)
        {
            if (dr.Table.Columns.Contains(columnName))
            {
                return dr[columnName].ToObject<T>();
            }
            return default(T);
        }

        /// <summary>
        /// 自动添加换行符
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void AppendFormatLine(this StringBuilder _this, string format, params object[] args)
        {
            _this.AppendFormat(format, args);
            _this.Append(Environment.NewLine);
        }
    }
}