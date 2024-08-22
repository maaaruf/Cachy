using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using System.Text.Json;

namespace Cachy;

public class CacheSet<T>
{
    private readonly IDistributedCache _cache;
    private readonly ICacheKeyFactory<T> _keyFactory;
    private readonly bool _hasGenerateSuffixMethod;

    public CacheSet(IDistributedCache cache, ICacheKeyFactory<T> keyFactory)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        _hasGenerateSuffixMethod = _hasGenerateSuffixMethod = GetExtensionMethods(typeof(CacheSet<T>))
                                       .Any(m => m.ReturnType == typeof(string) &&
                                                 m.GetParameters().Length > 0 &&
                                                 m.GetParameters().Any(x => x.ParameterType == typeof(CacheSet<T>)));
    }

    public async Task<T?> GetAsync(string suffix = "")
    {
        var key = GetKey(suffix);
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(data))
            return default;
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync(T value, string suffix = "" , DistributedCacheEntryOptions? options = null)
    {
        var key = GetKey(suffix);
        ValidateValue(value);

        var json = JsonSerializer.Serialize(value);
        if (options == null)
            await _cache.SetStringAsync(key, json);
        else
            await _cache.SetStringAsync(key, json, options);
    }

    private static void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
    }

    private static void ValidateValue(T value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
    }

    private string GetKey(string suffix)
    {
        if (_hasGenerateSuffixMethod && string.IsNullOrWhiteSpace(suffix))
        {
            throw new ArgumentException("Suffix is required for this cache");
        }

        var key = _keyFactory.GenerateKey();
        key = $"{key}_{suffix}";

        ValidateKey(key);

        return key;    
    }

    private static IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSealed && !type.IsGenericType && !type.IsNested)
                {
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length > 0 && parameters[0].ParameterType == extendedType)
                            {
                                yield return method;
                            }
                        }
                    }
                }
            }
        }
    }
}
