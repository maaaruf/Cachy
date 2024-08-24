using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Cachy;

public class CacheContext : MemoryDistributedCache
{
    public CacheContext(
        IOptions<MemoryDistributedCacheOptions> optionsAccessor,
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider)
    : base(optionsAccessor, loggerFactory)
    {
        // Initialize CacheSets based on the properties defined
        foreach (var prop in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(CacheSet<>))
            {
                var modelType = prop.PropertyType.GetGenericArguments()[0];
                var cacheKeyFactory = serviceProvider.GetService(typeof(ICacheKeyFactory<>).MakeGenericType(modelType));
                var cache = serviceProvider.GetService(typeof(IDistributedCache));

                var cacheSet = Activator.CreateInstance(prop.PropertyType, cache, cacheKeyFactory, loggerFactory);
                prop.SetValue(this, cacheSet);
            }
        }
    }
}
