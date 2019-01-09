using Newtonsoft.Json.Serialization;
using System;

namespace JW.RequestRelay.Util.Json
{
    /// <summary>
    /// Ajax 类库 Json 序列化设置，首字母小写，枚举转成整型
    /// </summary>
    public partial class AjaxJsonResolver : CamelCasePropertyNamesContractResolver
    {
        public AjaxJsonResolver() { }

        public override JsonContract ResolveContract(Type objectType)
        {
            var jsoncontract = base.ResolveContract(objectType);
            if (objectType.BaseType == typeof(Enum))
            {
                jsoncontract.Converter = new EnumJsonConvert();
            }
            if (objectType == typeof(DateTime))
            {
                jsoncontract.Converter = new DateTimeFormatConvert();
            }
            if (objectType == typeof(DateTime?))
            {
                jsoncontract.Converter = new DateTimeFormatConvert();
            }
            return jsoncontract;
        }
    }

}
