using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Common
{
    //
    // 摘要:
    //     缓存
    public class CacheTo
    {
        //
        // 摘要:
        //     缓存
        public static IMemoryCache memoryCache;

        //
        // 摘要:
        //     获取数据缓存
        //
        // 参数:
        //   CacheKey:
        //     键
        public static object Get(string CacheKey)
        {
            memoryCache.TryGetValue(CacheKey, out object value);
            return value;
        }

        //
        // 摘要:
        //     设置数据缓存 变化时间过期（平滑过期）。表示缓存连续2个小时没有访问就过期（TimeSpan.FromSeconds(7200)）。
        //
        // 参数:
        //   CacheKey:
        //     键
        //
        //   objObject:
        //     值
        //
        //   Second:
        //     过期时间，默认7200秒
        //
        //   Sliding:
        //     是否相对过期，默认是；否，则固定时间过期
        public static void Set(string CacheKey, object objObject, int Second = 7200, bool Sliding = true)
        {
            memoryCache.Set(CacheKey, objObject, Sliding ? new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(Second)) : new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Second)));
        }

        //
        // 摘要:
        //     移除指定数据缓存
        //
        // 参数:
        //   CacheKey:
        //     键
        public static void Remove(string CacheKey)
        {
            memoryCache.Remove(CacheKey);
        }

        //
        // 摘要:
        //     移除全部缓存
        public static void RemoveAll()
        {
            memoryCache.Dispose();
        }
    }
}
