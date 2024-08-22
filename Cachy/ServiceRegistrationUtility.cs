using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cachy;

public static class ServiceRegistrationUtility
{
    public static void RegisterImplementations(IServiceCollection services, Assembly assembly, Type serviceType, ServiceLifetime lifetime)
    {
        var types = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == serviceType))
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == serviceType).ToList();
            foreach (var @interface in interfaces)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(@interface, type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(@interface, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(@interface, type);
                        break;
                }
            }
        }
    }
}
