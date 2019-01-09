using JW.RequestRelay.Util.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;

namespace JW.RequestRelay.Util.Cache
{
    /// <summary>
    /// 缓存池
    /// </summary>
    public partial class CachePool<TKey, TValue>
    {
        /// <summary>
        /// 缓冲池 做为系统的一级缓存使用
        /// </summary>
        public ConcurrentDictionary<TKey, TValue> CACHE_POOL { get; private set; }

        /// <summary>
        /// 缓存存入时间
        /// </summary>
        public ConcurrentDictionary<TKey, DateTime> CACHE_POOL_CreateTime { get; private set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public ConcurrentDictionary<TKey, DateTime> CACHE_POOL_LastAccessTime { get; private set; }

        /// <summary>
        /// 一级缓存过期时间 单位分钟
        /// </summary>
        public int Timeout { get; private set; }

        /// <summary>
        /// 清理一级缓存的缓存数量阈值  0:表示自动，按照当前电脑内存情况自行判断
        /// </summary>
        public int CACHE_POOL_THRESHOLD { get; private set; }

        /// <summary>
        /// 过期类型
        /// </summary>
        public CacheExpireTypeEnum ExpireType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">过期时间 （单位分钟） 默认永不过期</param>
        /// <param name="size">缓存池大小</param>
        /// <param name="type">过期类型</param>
        public CachePool(int timeout = 0, int size = 0, CacheExpireTypeEnum type = 0)
        {
            this.Timeout = timeout;
            this.CACHE_POOL_THRESHOLD = size;
            this.ExpireType = type;
            CACHE_POOL = new ConcurrentDictionary<TKey, TValue>();
            if (this.Timeout > 0)
            {
                switch (type)
                {
                    case CacheExpireTypeEnum.Adjustable:
                        CACHE_POOL_LastAccessTime = new ConcurrentDictionary<TKey, DateTime>();
                        break;
                    case CacheExpireTypeEnum.Absolute:
                        CACHE_POOL_CreateTime = new ConcurrentDictionary<TKey, DateTime>();
                        break;
                }
            }
            if (size > 0 && CACHE_POOL_LastAccessTime != null)
            {
                CACHE_POOL_LastAccessTime = new ConcurrentDictionary<TKey, DateTime>();
            }
            if (size > 0 || timeout > 0)
            {
                GlobalTimerHelper.Instance.AddEvent(this.AutoClear);
            }
        }

        /// <summary>
        /// 添加/更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(TKey key, TValue value)
        {
            if (!IsDefault<TKey>(key))
            {
                if (CACHE_POOL.ContainsKey(key))
                {
                    CACHE_POOL[key] = value;
                    if (CACHE_POOL_CreateTime != null)
                    {
                        CACHE_POOL_CreateTime[key] = DateTime.Now;
                    }
                }
                else
                {
                    CACHE_POOL.TryAdd(key, value);
                    if (CACHE_POOL_CreateTime != null)
                    {
                        CACHE_POOL_CreateTime.TryAdd(key, DateTime.Now);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public void SetAll(IDictionary<TKey, TValue> collection)
        {
            foreach (var item in collection)
            {
                Set(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 移出缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Remove(TKey key)
        {
            TValue value;
            CACHE_POOL.TryRemove(key, out value);
            if (CACHE_POOL_LastAccessTime != null)
            {
                DateTime lastAccessTime;
                CACHE_POOL_LastAccessTime.TryRemove(key, out lastAccessTime);
            }
            if (CACHE_POOL_CreateTime != null)
            {
                DateTime createTime;
                CACHE_POOL_CreateTime.TryRemove(key, out createTime);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<TValue> RemoveAll(List<TKey> keys)
        {
            var ret = new List<TValue>(keys.Count);
            foreach (TKey key in keys)
            {
                var item = Remove(key);
                if (item != null)
                {
                    ret.Add(item);
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            if (!IsDefault<TKey>(key))
            {
                if (CACHE_POOL.ContainsKey(key))
                {
                    if (CACHE_POOL_LastAccessTime != null)
                    {
                        if (CACHE_POOL_LastAccessTime.ContainsKey(key))
                        {
                            CACHE_POOL_LastAccessTime[key] = DateTime.Now;
                        }
                        else
                        {
                            CACHE_POOL_LastAccessTime.TryAdd(key, DateTime.Now);
                        }
                    }
                    return CACHE_POOL[key];
                }
            }
            return default(TValue);
        }

        public TValue this[TKey key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        /// <summary>
        /// 获取多个缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IDictionary<TKey, TValue> GetAll(List<TKey> keys)
        {
            IDictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(keys.Count);
            foreach (var key in keys)
            {
                if (key == null)
                {
                    continue;
                }
                if (!ret.ContainsKey(key))
                {
                    var item = Get(key);
                    if (item != null)
                    {
                        ret.Add(key, item);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 判断指定KEY是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return CACHE_POOL.ContainsKey(key);
        }

        /// <summary>
        /// 清除全部
        /// </summary>
        public void Clear()
        {
            CACHE_POOL.Clear();
            if (this.Timeout > 0)
            {
                CACHE_POOL_CreateTime.Clear();
            }
        }

        /// <summary>
        /// 判断key是不是默认值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsDefault<T>(TKey key)
        {
            if (key == null)
            {
                return true;
            }
            if (key is string)
            {
                return string.IsNullOrEmpty(key as string);
            }
            if (key.IsNumber())
            {
                return key.Equals(0);
            }
            return false;
        }

        /// <summary>
        /// 自动清理缓存项
        /// </summary>
        public void AutoClear(object sender, ElapsedEventArgs e)
        {
            try
            {
                var nowTime = DateTime.Now;
                if (this.Timeout > 0)
                {
                    if (this.ExpireType == CacheExpireTypeEnum.Absolute)
                    {
                        foreach (var cache in this.CACHE_POOL)
                        {
                            DateTime createTime;
                            if (this.CACHE_POOL_CreateTime.TryGetValue(cache.Key, out createTime))
                            {
                                if (createTime.AddMinutes(this.Timeout) < nowTime)
                                {
                                    this.Remove(cache.Key);
                                }
                            }
                        }
                    }
                    if (this.ExpireType == CacheExpireTypeEnum.Adjustable)
                    {
                        foreach (var cache in this.CACHE_POOL)
                        {
                            DateTime lastAccessTime;
                            if (this.CACHE_POOL_LastAccessTime.TryGetValue(cache.Key, out lastAccessTime))
                            {
                                if (lastAccessTime.AddMinutes(this.Timeout) < nowTime)
                                {
                                    this.Remove(cache.Key);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Fatal("缓存自动清理异常", ex);
            }
        }
    }

    /// <summary>
    /// 缓存过期类型
    /// </summary>
    public enum CacheExpireTypeEnum
    {
        /// <summary>
        /// 绝对过期(到时间就过期)
        /// </summary>
        Absolute = 1,
        /// <summary>
        /// 可调过期（距离最后一次访问时间过期）
        /// </summary>
        Adjustable = 2,
    }
}
