using Cachy.WebApi.Contexts;
using Cachy.WebApi.Models.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cachy.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly MyCacheContext _cacheContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, MyCacheContext cacheContext)
        {
            _logger = logger;
            _cacheContext = cacheContext;
        }

        [HttpPost(Name = "SetCache")]
        public async Task<IActionResult> SetAsync()
        {
            var balance = new BalanceCache
            {
                Balance = new Random().Next(10, 100),
                LastUpdated = DateTime.Now,
            };

            var suffix = _cacheContext.Balances.TestSuffix("Something", "userId");
            var prefix = _cacheContext.Balances.GeneratePrefix("5");

            await _cacheContext.Balances.SetAsync(balance, prefix, suffix);

            return Ok(balance);
        }

        [HttpGet(Name = "GetCache")]
        public async Task<BalanceCache> GetAsync()
        {
            var balance = new BalanceCache
            {
                 Balance = 0,
                 LastUpdated = DateTime.UtcNow,
            };

            var prefix = _cacheContext.Balances.GeneratePrefix("5");
            var suffix = _cacheContext.Balances.TestSuffix("Something", "userId");
            var data = await _cacheContext.Balances.GetAsync(prefix, suffix);

            return data;
        }
    }
}
