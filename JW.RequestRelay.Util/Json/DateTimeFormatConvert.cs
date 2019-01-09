using Newtonsoft.Json;
using System;

namespace JW.RequestRelay.Util.Json
{
    public partial class DateTimeFormatConvert : JsonConverter
    {
        private string Format;
        public DateTimeFormatConvert()
        {

        }

        public DateTimeFormatConvert(string format)
        {
            this.Format = format;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.Value == DBNull.Value)
            {
                return null;
            }
            try
            {
                return reader.Value.ToDateTimeCanNull();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("不能将时间{1}的值{0}转换为Json格式.", reader.Value, objectType));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var _value = (DateTime)value;
            if (_value == default(DateTime))
            {
                writer.WriteNull();
                return;
            }
            if (string.IsNullOrEmpty(this.Format))
            {
                if (_value.Second == 0)
                {
                    if (_value.Hour == 0 && _value.Minute == 0)
                    {
                        writer.WriteValue(_value.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        writer.WriteValue(_value.ToString("yyyy-MM-dd HH:mm"));
                    }
                }
                else
                {
                    writer.WriteValue(_value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else
            {
                writer.WriteValue(_value.ToString(this.Format));
            }
        }
    }
}
