﻿using System.Collections.Generic;
using System.Collections.Specialized;

namespace System
{
    /// <summary>
    /// Extension methods for Dictionary.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// This method is used to try to get a value in a dictionary if it does exists.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="dictionary">The collection object</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value of the key (or default value if key not exists)</param>
        /// <returns>True if key does exists in the dictionary</returns>
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object valueObj;
            if (dictionary.TryGetValue(key, out valueObj) && valueObj is T)
            {
                value = (T)valueObj;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            return dictionary.TryGetValue(key, out obj) ? obj : default(TValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection _this)
        {
            var ret = new Dictionary<string, string>();
            foreach (var key in _this.AllKeys)
            {
                ret.Add(key, _this.Get(key));
            }
            return ret;
        }
    }
}