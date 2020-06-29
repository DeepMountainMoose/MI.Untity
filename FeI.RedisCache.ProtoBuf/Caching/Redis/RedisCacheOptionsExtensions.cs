using MI.Core.Dependency;
using MI.RedisCache.Caching.Redis;
using MI.Core.Configuration.Startup;

namespace FeI.RedisCache.ProtoBuf.Caching.Redis
{
    public static class RedisCacheOptionsExtensions
    {
        public static void UseProtoBuf(this RedisCacheOptions options)
        {
            options.StartupConfiguration
                .ReplaceService<IRedisCacheSerializer, ProtoBufRedisCacheSerializer>(DependencyLifeStyle.Transient);
        }
    }
}
