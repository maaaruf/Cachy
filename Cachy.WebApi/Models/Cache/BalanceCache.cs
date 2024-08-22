using System.Runtime.CompilerServices;

namespace Cachy.WebApi.Models.Cache;

public class BalanceCache
{
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }
}

public static class BalanceCacheExtensions
{
    public static string TestSuffix(this CacheSet<BalanceCache> cacheSet, string financialEntityId, string userId)
    {
        // Example suffix generation logic
        return $"_{financialEntityId}_{userId}";
    }
}