﻿using Microsoft.Extensions.DependencyInjection;

namespace Cachy
{
    public static class Extensions
    {
        public static IServiceCollection AddCacheContext<T>(this IServiceCollection services) where T : CacheContext
        {
            services.AddDistributedMemoryCache();

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
