using System;
using System.Threading.Tasks;

namespace Common.CacheMemory
{
    public interface ICacheMemory
    {
        Task Set<T>(string key, T item, CacheTime time = CacheTime.Normal, Provider? provider = null);
        Task<T> Get<T>(string key, Provider? provider = null);
        Task<object> Get(Type type, string key, Provider? provider = null);
        Task Remove<T>(string key);
        Task Remove(string key);
        Task RemoveAll<T>(string key);
        Task RemoveAll(string key);
        Task RemoveAll();
        bool Exists<T>(string key);
    }
}