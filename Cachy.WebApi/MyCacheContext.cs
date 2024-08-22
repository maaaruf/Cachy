using Cachy.WebApi.Models.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Cachy.WebApi;

public class MyCacheContext : CacheContext
{
    public CacheSet<BalanceCache> Balances { get; set; } = null!;

    public MyCacheContext(
        IOptions<MemoryDistributedCacheOptions> optionsAccessor,
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider)
        : base(optionsAccessor, loggerFactory, serviceProvider)
    {
    }
}
