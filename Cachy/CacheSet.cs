using Cachy.DataTypes;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Cachy;

public class CacheSet<T>
{
    private readonly IDistributedCache _cache;
    private readonly ICacheKeyFactory<T> _keyFactory;
    private readonly IList<MethodInfo> _suffixMethods;
    private readonly IList<MethodInfo> _prefixMethods;
    private readonly ILogger<CacheSet<T>> _logger;

    public CacheSet(IDistributedCache cache, ICacheKeyFactory<T> keyFactory, ILoggerFactory loggerFactory)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _keyFactory = keyFactory;
        _logger = loggerFactory.CreateLogger<CacheSet<T>>();

        var extendedMethods = Helpers.GetExtensionMethods(typeof(CacheSet<T>));
        
        _suffixMethods = extendedMethods.Where(m => m.ReturnType == typeof(Suffix) &&
                        m.GetParameters().Length > 0 &&
                        m.GetParameters().Any(x => x.ParameterType == typeof(CacheSet<T>))).ToList();

        _prefixMethods = extendedMethods.Where(m => m.ReturnType == typeof(Suffix) &&
                        m.GetParameters().Length > 0 &&
                        m.GetParameters().Any(x => x.ParameterType == typeof(CacheSet<T>))).ToList();
    }

    public async Task<T?> GetAsync(Prefix prefix = default, Suffix suffix = default, CancellationToken token = default)
    {
        var key = GetKey(prefix, suffix);
        _logger?.LogInformation("Retrieving data with key: {Key}", key);

        var data = await _cache.GetStringAsync(key, token);

        if (string.IsNullOrWhiteSpace(data))
        {
            _logger?.LogWarning("No data found for key: {Key}", key);
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync(T value, Prefix prefix = default, Suffix suffix = default, DistributedCacheEntryOptions? options = null)
    {
        var key = GetKey(prefix, suffix);
        ValidateValue(value);

        var json = JsonSerializer.Serialize(value);
        _logger?.LogInformation("Setting data with key: {Key}", key);

        if (options == null)
            await _cache.SetStringAsync(key, json);
        else
            await _cache.SetStringAsync(key, json, options);

        _logger?.LogInformation("Data set with key: {Key}", key);
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

    protected string GetKey(Prefix prefix, Suffix suffix)
    {
        if (_suffixMethods.Any() && string.IsNullOrWhiteSpace(suffix.ToString()))
        {
            var methodNames = GetMethodNames(_suffixMethods);
            
            string message = $"Cache suffix is required for {typeof(T).Name}. Available methods to generate suffix: {methodNames}";
            
            _logger?.LogError(message);
            throw new ArgumentException(message);
        }

        if (_prefixMethods.Any() && string.IsNullOrWhiteSpace(prefix.ToString()))
        {
            var methodNames = GetMethodNames(_prefixMethods);

            string message = $"Prefix is required for this cache. Available methods to generate prefix: {methodNames}";
            
            _logger?.LogError(message);
            throw new ArgumentException(message);
        }

        string key = _keyFactory != null ? _keyFactory.GenerateKey() : typeof(T).Name;
        key = $"{key}_{suffix}";

        ValidateKey(key);

        return key;
    }

    private string GetMethodNames(IList<MethodInfo> methodInfos)
    {
        return string.Join(", ", methodInfos.Select(m =>
            $"{m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})"));
    }
}
