using MI.Core.Modules;
using MI.Core.Reflection;

namespace MI.RedisCache.Caching.Redis
{
    [DependsOn(typeof(KernelModule))]
    public class RedisCacheModule : Module
    {
        public override void PreInitialize()
        {
            IocManager.RegisterType<RedisCacheOptions>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RedisCacheModule).GetAssembly());
        }
    }
}
