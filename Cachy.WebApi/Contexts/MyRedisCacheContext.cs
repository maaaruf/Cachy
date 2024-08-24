//using Cachy.StackExchange.Redis;
//using Cachy.WebApi.Models.Cache;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Caching.StackExchangeRedis;
//using Microsoft.Extensions.Options;
//using StackExchange.Redis;

//namespace Cachy.WebApi.Contexts;

//public class MyRedisCacheContext : RedisCacheContext
//{
//    public RedisCacheSet<BalanceCache> Balances { get; set; } = null!;

//    public MyRedisCacheContext(
//        IServiceProvider serviceProvider,
//        IConnectionMultiplexer redisConnection)
//        : base(serviceProvider, redisConnection)
//    {
        
//    }
//}
