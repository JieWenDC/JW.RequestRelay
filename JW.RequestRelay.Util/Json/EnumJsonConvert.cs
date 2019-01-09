using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace JW.RequestRelay.Util.Json
{
    public partial class EnumJsonConvert : JsonConverter
    {
        public void EnumConverter()
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.Value == null)
            {
                return null;
            }
            try
            {
                if (reader.Value.IsNumber())
                {
                    return Enum.ToObject(objectType, reader.Value.ToInt());
                }
                if (reader.Value.GetType().FullName == "System.String")
                {
                    var value = reader.Value as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value.StartsWith("JSON="))
                        {
                            value = value.TrimStart("JSON=");
                            var json = value.ToObject<JObject>();
                            return Enum.ToObject(objectType, json.Value<int>("value"));
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                return Enum.Parse(objectType, reader.Value.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("不能将{0}转换为{1}类型", reader.Value, objectType));
            }

        }

        /// <summary>
        /// 判断是否为Bool类型
        /// </summary>
        /// <param name="objectType">类型</param>
        /// <returns>为bool类型则可以进行转换</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }


        public bool IsNullableType(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return t.BaseType != null && (t.BaseType.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            if (writer.Path == "code")
            {
                writer.WriteValue(value);
            }
            else {
                string text = EnumHelper.GetDes(value as ValueType);
                writer.WriteValue(string.Format("JSON={{\"text\":\"{0}\",\"value\":{1},\"key\":\"{2}\"}}", text, (int)value, value.ToString()));
            }
        }
    }
}
