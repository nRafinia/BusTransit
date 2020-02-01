using System.Threading.Tasks;
using Refit;

namespace Common.CacheMemory
{
    public interface ICacheService
    {
        [Get("/cache/Get/{provider}/{key}")]
        Task<CacheResponse> Get(Provider provider, string key);

        [Post("/cache/Set/{provider}/{key}")]
        Task Set(Provider provider, string key,[Body] SetCacheModel data);
    }
}