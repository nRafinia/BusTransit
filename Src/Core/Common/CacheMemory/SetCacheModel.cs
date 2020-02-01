using System;

namespace Common.CacheMemory
{
    public class SetCacheModel
    {
        public string Item { get; set; }
        public TimeSpan ExpireTime { get; set; }
    }
}