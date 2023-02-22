using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace SimpleDLock.Core.Utils
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisConnection(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.AddSingleton(provider =>
            {
                var endpoints = configuration.GetSection("RedisEndpoints").Get<IEnumerable<string>>();
                return RedisHelper.GetConnectionMultiplexer(endpoints.First());
            });
        }

        public static IServiceCollection AddRedLock(this IServiceCollection services)
        {
            return services.AddSingleton(provider =>
            {
                var multiplexer = provider.GetRequiredService<ConnectionMultiplexer>();
                var redLockMultiplexers = new List<RedLockMultiplexer>()
                {
                    multiplexer
                };
                return RedLockFactory.Create(redLockMultiplexers);
            });
        }
    }
}
