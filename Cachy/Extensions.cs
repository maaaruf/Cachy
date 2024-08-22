using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cachy
{
    public static class Extensions
    {
        private static void AddCachy(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo(typeof(ICacheKeyFactory<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            //ServiceRegistrationUtility.RegisterImplementations(
            //    services,
            //    Assembly.GetExecutingAssembly(),
            //    typeof(ICacheKeyFactory<>),
            //    ServiceLifetime.Singleton
            //);
        }

        public static IServiceCollection AddCacheContext<T>(this IServiceCollection services) where T : CacheContext
        {
            AddCachy(services);
            services.AddSingleton<T>();

            return services;
        }
    }
}
