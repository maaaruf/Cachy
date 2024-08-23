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
        _hasGenerateSuffixMethod = Helpers.GetExtensionMethods(typeof(CacheSet<T>))
            .Any(m => m.ReturnType == typeof(string) &&
                        m.GetParameters().Length > 0 &&
                        m.GetParameters().Any(x => x.ParameterType == typeof(CacheSet<T>)));
    }

    public async Task<T?> GetAsync(string? suffix, CancellationToken token = default)
    {
        var key = GetKey(suffix);
        var data = await _cache.GetStringAsync(key, token);
        
        if (string.IsNullOrWhiteSpace(data))
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync(string? suffix, T value, DistributedCacheEntryOptions? options = null)
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

    protected string GetKey(string? suffix)
    {
        if (_hasGenerateSuffixMethod && string.IsNullOrWhiteSpace(suffix))
        {
            throw new ArgumentException("Suffix is required for this cache");
        }

        var key = _keyFactory is not null ? _keyFactory.GenerateKey() : nameof(T);
        key = $"{key}_{suffix}";

        ValidateKey(key);

        return key;
    }
}
