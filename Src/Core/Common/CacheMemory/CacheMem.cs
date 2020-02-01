using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Common.CacheMemory
{
    public class CacheMem : ICacheMemory
    {
        private readonly IInitProject _initProject;
        private readonly ICacheService _cache;

        public CacheMem(IInitProject initProvider, ICacheService cacheService)
        {
            _initProject = initProvider;
            _cache = cacheService;
        }

        private string GetKeyName(Type type, string key)
        {
            var prefix = type.FullName;

            return $"{prefix}:{key}";
        }

        private string GetKeyName<T>(string key)
        {
            return GetKeyName(typeof(T), key);
        }

        public async Task Set<T>(string key, T item, CacheTime time = CacheTime.Normal, Provider? provider = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key) || EqualityComparer<T>.Default.Equals(item, default(T)))
                    return;

                var itemStr = JsonSerializer.Serialize(item);
                key = GetKeyName(item.GetType(), key);
                await _cache.Set(provider ?? _initProject.Provider, key, new SetCacheModel()
                {
                    Item = itemStr,
                    ExpireTime = TimeSpan.FromSeconds((int)time)
                });

            }
            catch (Exception e)
            {
                //_logger.Error(e);
            }
        }

        public async Task<T> Get<T>(string key, Provider? provider = null)
        {
            var res = await Get(typeof(T), key, provider);

            return res == null
                ? default
                : (T)res;
        }

        public async Task<object> Get(Type type, string key, Provider? provider = null)
        {
            object res = null;
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;

                key = GetKeyName(type, key);

                var r = await _cache.Get(provider ?? _initProject.Provider, key);

                if (r == null || r?.ExpireDate < DateTime.Now)
                    return null;

                res = JsonSerializer.Deserialize(r?.Data ?? string.Empty, type);
                //res=JsonConvert.DeserializeObject(r?.Data ?? string.Empty, type);
                //res =((JObject) JsonConvert.DeserializeObject(r?.Data)).ToObject(type);
            }
            catch (Exception e)
            {
                //_logger.Error(e, new {key, res1});
            }

            return res;
        }

        public Task Remove<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAll<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAll(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAll()
        {
            throw new NotImplementedException();
        }

        public bool Exists<T>(string key)
        {
            throw new NotImplementedException();
        }
    }
}