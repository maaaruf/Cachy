using Cachy.DataTypes;
using System.Runtime.CompilerServices;

namespace Cachy.WebApi.Models.Cache;

public class BalanceCache
{
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }
}

public static class BalanceCacheExtensions
{
    public static Prefix GeneratePrefix(this CacheSet<BalanceCache> cacheSet, string organizationId)
    {
        return new Prefix($"_{organizationId}");
    }

    public static Suffix TestSuffix(this CacheSet<BalanceCache> cacheSet, string financialEntityId, string userId)
    {
        return new Suffix($"_{financialEntityId}_{userId}");
    }
}