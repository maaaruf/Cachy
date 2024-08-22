using Cachy.WebApi.Models.Cache;

namespace Cachy.WebApi.KeyFactory
{
    public class BalanceCacheKeyFactory : ICacheKeyFactory<BalanceCache>
    {
        public string GenerateKey() => "Balance";
    }
}
