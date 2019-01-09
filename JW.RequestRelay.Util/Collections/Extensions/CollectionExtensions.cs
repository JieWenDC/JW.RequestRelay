using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System
{
    /// <summary>
    /// ������չ
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// ��鼯���Ƿ��������
        /// </summary>
        public static bool ExistsData<T>(this ICollection<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        /// <summary>
        /// ��鼯���Ƿ�Ϊ��ΪNULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// ��鼯���Ƿ�δ��δNUll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void CheckIsNullOrEmpty<T>(this ICollection<T> source, string msg)
        {
            source.IsNullOrEmpty<T>().CheckIsTrue(msg);
        }

        /// <summary>
        /// ������ӵ����ϣ�����������Ѿ����ռ���
        /// </summary>
        /// <param name="source">����</param>
        /// <param name="item">��</param>
        /// <typeparam name="T">����/typeparam>
        /// <returns>������أ�����true�����������false</returns>
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
        /// ��ȡ����
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
        /// �Զ���ӻ��з�
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