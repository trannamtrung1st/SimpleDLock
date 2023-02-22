using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

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
    }
}
