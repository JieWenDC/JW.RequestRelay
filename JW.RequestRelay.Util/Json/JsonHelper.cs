using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JW.RequestRelay.Util.Json
{
    public partial class JsonHelper
    {
        /// <summary>
        /// 转换为JSON
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="camelCase">驼峰转换</param>
        /// <param name="indented">是否缩进</param>
        /// <returns></returns>
        public static string SerializeObject(object value, bool format = false, bool camelCase = false, bool ignoreNull = false)
        {
            var options = new JsonSerializerSettings();
            options.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            if (format)
            {
                options.ContractResolver = new AjaxJsonResolver();
                options.NullValueHandling = NullValueHandling.Ignore;
            }
            else
            {
                if (camelCase)
                {
                    options.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
                if (ignoreNull)
                {
                    options.NullValueHandling = NullValueHandling.Ignore;
                }
            }
            return JsonConvert.SerializeObject(value, options);

        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEntity DeserializeObject<TEntity>(string value)
        {
            return (TEntity)ReflectionHelper.ChangeType(value, typeof(TEntity));
        }
    }
}
