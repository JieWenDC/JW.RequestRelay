using JW.RequestRelay.Util.Cache;
using JW.RequestRelay.Util.Json;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace JW.RequestRelay.Util
{
    public partial class ReflectionHelper
    {
        /// <summary>
        /// 条件反射程序集缓存
        /// </summary>
        private static CachePool<string, Assembly> CACHE_ASSEMBLY = new CachePool<string, Assembly>();

        /// <summary>
        /// 方法参数缓存
        /// </summary>
        private static CachePool<MethodInfo, ParameterInfo[]> CACHE_METHOD_PARAM = new CachePool<MethodInfo, ParameterInfo[]>();

        /// <summary>
        /// 缓存方法信息
        /// </summary>
        private static CachePool<string, MethodInfo> CACHE_METHOD = new CachePool<string, MethodInfo>();

        /// <summary>
        /// 获取指定程序集
        /// </summary>
        /// <param name="assemblyString"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string assemblyString)
        {
            Assembly assembly = CACHE_ASSEMBLY[assemblyString];
            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyString);
                CACHE_ASSEMBLY[assemblyString] = assembly;
            }
            return assembly;
        }

        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Type classType, string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullException("方法名不能为空");
            }
            var key = string.Format("{0}-{1}", classType.FullName, methodName);
            MethodInfo method = CACHE_METHOD[key];
            if (method == null)
            {
                method = classType.GetMethod(methodName);
                CACHE_METHOD[key] = method;
            }
            return method;
        }

        /// <summary>
        /// 获取指定方法参数
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ParameterInfo[] GetMethodParamList(MethodInfo method)
        {
            ParameterInfo[] parameters = CACHE_METHOD_PARAM[method];
            if (parameters == null)
            {
                parameters = method.GetParameters();
                CACHE_METHOD_PARAM[method] = parameters;
            }
            return parameters;
        }

        /// <summary>
        /// 把对象转换未指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type conversionType)
        {

            if (value == null || string.IsNullOrEmpty(value as string))
            {
                return DefaultForType(conversionType);
            }
            var inType = value.GetType();
            if (inType == conversionType)
            {
                return value;
            }
            // 如果输入类型为可空类型，改为其基类型
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }

            if (conversionType == typeof(bool))
            {
                return value.ToBool();
            }
            if (conversionType.IsEnum)
            {
                if (value.IsNumber())
                {
                    return System.Enum.ToObject(conversionType, value.ToInt());
                }
                else
                {
                    return System.Enum.Parse(conversionType, value.ToString());
                }
            }
            if (conversionType.FullName == "System.Object" && value is string)
            {
                return value;
            }
            var setting = new JsonSerializerSettings()
            {
                ContractResolver = new AjaxJsonResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
            };
            return conversionType.GetInterface("IConvertible") == null ?
                JsonConvert.DeserializeObject(value.ToString(), conversionType, setting)
                : Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// 获取指定类型的所有公共属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// 执行指定方法
        /// </summary>
        /// <param name="assemblyString"></param>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeMethod(string assemblyString, string typeName, string methodName, object[] parameters = null)
        {
            var assembly = GetAssembly(assemblyString);
            var instance = assembly.CreateInstance(typeName, false);

            var method = GetMethodInfo(instance.GetType(), methodName);
            return method.Invoke(instance, parameters);
        }

        /// <summary>
        /// 执行静态方法
        /// </summary>
        /// <param name="assemblyString"></param>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeStaticMethod(string assemblyString, string typeName, string methodName, object[] parameters = null)
        {
            var assembly = GetAssembly(assemblyString);
            var type = assembly.GetType(typeName);
            var propertyInfo = type.GetProperty("Instance");
            if (propertyInfo != null)
            {
                var instance = propertyInfo.GetValue(null);
                var method = GetMethodInfo(instance.GetType(), methodName);
                return method.Invoke(instance, parameters);
            }
            else
            {
                return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, null, parameters);
            }
        }

    }
}
