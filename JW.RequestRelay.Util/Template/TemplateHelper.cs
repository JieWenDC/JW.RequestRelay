using JW.RequestRelay.Util.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JW.RequestRelay.Util.Template
{
    public partial class TemplateHelper
    {

        /// <summary>
        /// 解析（替换）字符串中的表单数据
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="formData">表单数据</param>
        /// <returns></returns>
        public static string ParsingStringFormData(string str, Dictionary<string, object> formData)
        {
            if (!formData.ExistsData() || string.IsNullOrEmpty(str))
            {
                return str;
            }
            //查找字符串中的{{=Str}}
            var reg_interpolate = new Regex("{(((?!{).)+?)}");
            var matchCollection = reg_interpolate.Matches(str);
            var ret_str = str;
            foreach (Match match in matchCollection)
            {
                var key = match.Value.TrimStart('{').TrimEnd('}');
                var isJson = false;
                if (key.StartsWith("(JSON)"))
                {
                    key = key.TrimStart("(JSON)");
                    isJson = true;
                }
                object form_value = null;
                if (formData.ContainsKey(key))
                {
                    form_value = formData[key];
                }
                else
                {
                    if (key.IndexOf(".") > -1)
                    {
                        var field_tree = key.Split('.');
                        var formData_obj_key = field_tree[0];
                        if (formData.ContainsKey(formData_obj_key))
                        {
                            form_value = formData[formData_obj_key];
                            for (var i = 1; i < field_tree.Length; i++)
                            {
                                form_value = GetObjectFieldValue(form_value, field_tree[i]);
                            }
                        }
                    }
                }
                if (form_value != null)
                {
                    var ret_form_value = string.Empty;
                    if (isJson)
                    {
                        ret_form_value = JsonHelper.SerializeObject(form_value);
                    }
                    else
                    {
                        ret_form_value = form_value.ToString();
                    }
                    ret_str = ret_str.Replace(match.Value, ret_form_value);
                }
            }
            return ret_str.HtmlEncode();
        }

        /// <summary>
        /// 获取指定对象的指定字段值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object GetObjectFieldValue(object obj, string field)
        {
            if (field.IndexOf(".") > 0)
            {
                var property_tree = field.Split('.');
                for (var i = 0; i < property_tree.Length; i++)
                {
                    var property = property_tree[i];
                    var value = GetObjectPropertyValue(obj, property);
                    if (i == property_tree.Length - 1)
                    {
                        return value;
                    }
                    else
                    {
                        return GetObjectFieldValue(value, string.Join(".", property_tree.Skip(i + 1)));
                    }
                }
                return null;
            }
            else
            {
                return GetObjectPropertyValue(obj, field);
            }
        }

        /// <summary>
        /// 获取指定对象的指定属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetObjectPropertyValue(object obj, string propertyName)
        {
            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                dict.Contains(propertyName).CheckIsFalse(string.Format("IDictionary集合中不包含key为{0}的数据", propertyName));
                return dict[propertyName];
            }
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            propertyInfo.CheckNull(string.Format("属性{0}不在对象内", propertyName));
            return propertyInfo.GetValue(obj, null);
        }
    }
}
