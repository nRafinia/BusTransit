using System;
using Common.CacheMemory;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CacheOutputAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the cache key prefix. 
        /// </summary>
        /// <value>The cache key prefix.</value>
        public string CacheKeyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiration. The default value is 30 second.
        /// </summary>
        /// <value>The expiration.</value>
        public CacheTime Expiration { get; set; } = CacheTime.Minutes30;
    }
}