using JW.RequestRelay.Util.Cache;
using JW.RequestRelay.Util.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace JW.RequestRelay.Util
{
    public partial class EnumHelper
    {
        private static CachePool<Type, Dictionary<ValueType, string>> CACHE_TYPE = new CachePool<Type, Dictionary<ValueType, string>>();

        private static string LOCK = string.Empty;
        //是否已初始化
        private static bool isInit = false;

        /// <summary>
        /// 获取指定枚举的所有字段值以及描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<ValueType, string> GetList(Type type)
        {
            if (CACHE_TYPE.ContainsKey(type))
            {
                return CACHE_TYPE[type];
            }
            else
            {
                var ret = new Dictionary<ValueType, string>();
                var fieldInfo = type.GetFields();
                foreach (var field in fieldInfo)
                {
                    var atts = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    foreach (DescriptionAttribute att in atts)
                    {
                        var value = field.GetValue(type) as ValueType;
                        if (ret.ContainsKey(value))
                        {
                            Log4netHelper.Fatal(string.Format("类型{0}拥有相同的Value{1}", type.FullName, value));
                        }
                        else
                        {
                            ret.Add(value, att.Description);
                        }
                        break;
                    }
                }
                CACHE_TYPE[type] = ret;
                return ret;
            }
        }

        /// <summary>
        /// 获取所有的枚举项
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ValueType> GetEnumItemList(Type type)
        {
            var dict = GetList(type);
            var ret = new List<ValueType>();
            foreach (var item in dict)
            {
                ret.Add(item.Key);
            }
            return ret;
        }

        /// <summary>
        /// 获取枚举的序列化对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetListJson(Type type)
        {
            var ret = GetList(type);
            return ret.Select(row => new { Key = row.Key.ToString(), Value = (int)row.Key, Text = row.Value }).ToList();
        }

        /// <summary>
        /// 获取指定枚举的描述
        /// </summary>
        /// <param name="vt"></param>
        /// <returns></returns>
        public static string GetDes(ValueType vt)
        {
            var list = GetList(vt.GetType());
            foreach (var item in list)
            {
                if ((int)item.Key == (int)vt)
                {
                    return item.Value;
                }
            }
            return string.Empty;

        }

        /// <summary>
        /// 初始化全部
        /// </summary>
        /// <param name="assemblyString"></param>
        public static void Init(string assemblyString)
        {
            if (!isInit)
            {
                lock (LOCK)
                {
                    if (!isInit)
                    {
                        isInit = true;
                        var enum_assembly = Assembly.Load(assemblyString);
                        var enum_types = enum_assembly.GetTypes();
                        foreach (var type in enum_types)
                        {
                            var ret = GetList(type);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 根据描述获取枚举值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static ValueType GetValueByDesc(Type type, string desc)
        {
            var items = GetList(type);
            foreach (var item in items)
            {
                if (item.Value == desc)
                {
                    return item.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定枚举的描述列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetDescList(Type type)
        {
            var items = GetList(type);
            return items.Values.ToList();
        }
    }
}
