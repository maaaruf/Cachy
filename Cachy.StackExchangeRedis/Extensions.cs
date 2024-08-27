using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace Cachy.StackExchangeRedis
{
    public static class Extensions
    {
        public static IServiceCollection AddRedisCacheContext<T>(this IServiceCollection services, Action<RedisCacheOptions> options) where T : CacheContext
        {
            services.AddStackExchangeRedisCache(options);
            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo(typeof(ICacheKeyFactory<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            services.AddSingleton<T>();

            return services;
        }
    }
}
