using JW.RequestRelay.Util.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace JW.RequestRelay.Util
{
    /// <summary>
    /// 获取数据库实体的Description特性描述
    /// </summary>
    public partial class EntityHelper
    {
        private static CachePool<Type, Dictionary<string, string>> CACHE_TYPE = new CachePool<Type, Dictionary<string, string>>();

        private static string Entity_Name_Key = "EntityDesc";
        private static string LOCK = string.Empty;
        //是否已初始化
        private static bool isInit = false;
        /// <summary>
        /// 获取指定类型的所有字段值以及描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetList(Type type)
        {
            if (CACHE_TYPE.ContainsKey(type))
            {
                return CACHE_TYPE[type];
            }
            else
            {
                var entity_desc = new Dictionary<string, string>();
                var entity_att = type.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
                if (entity_att != null)
                {
                    entity_desc.Add(Entity_Name_Key, entity_att.Description);
                }

                var propertyInfos = type.GetProperties();
                foreach (var pf in propertyInfos)
                {
                    var att = pf.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (att != null)
                    {
                        entity_desc.Add(pf.Name, att.Description);
                    }
                }
                CACHE_TYPE[type] = entity_desc;
                return entity_desc;
            }
        }

        /// <summary>
        /// 获取指定类的描述
        /// </summary>
        /// <param name="vt"></param>
        /// <returns></returns>
        public static string GetDes(Type type, string fieldName = null)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = Entity_Name_Key;
            }
            var entity_desc = GetList(type);
            if (entity_desc.ContainsKey(fieldName))
            {
                return entity_desc[fieldName];
            }
            // throw new Exception(string.Format("{0}.{1}未使用特性DescriptionAttribute", entity, fieldName));
            return string.Empty;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (!isInit)
            {
                lock (LOCK)
                {
                    if (!isInit)
                    {
                        isInit = true;
                        var sql_assembly = Assembly.Load("LJ.UMFramework3.Entity.SQL");
                        var sql_types = sql_assembly.GetTypes().ToList();
                        var mongo_assembly = Assembly.Load("LJ.UMFramework3.Entity.Mongo");
                        var mongo_types = mongo_assembly.GetTypes().ToList();
                        sql_types.AddRange(mongo_types);
                        foreach (var type in sql_types)
                        {
                            if (type.GetCustomAttribute(typeof(DescriptionAttribute)) != null)
                            {
                                var dict_desc = GetList(type);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllEntity()
        {
            var ret = new Dictionary<string, string>();
            foreach (var type in CACHE_TYPE.CACHE_POOL)
            {
                var entity_name = type.Value.GetValue(Entity_Name_Key);
                var entity_code = type.Key.FullName;
                ret.Add(entity_code, entity_name);
            }
            return ret;
        }
    }
}
