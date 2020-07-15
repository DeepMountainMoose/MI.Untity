using MI.Core.Modules;
using MI.Core.Reflection;

namespace MI.EntityFramework.Common
{
    [DependsOn(typeof(KernelModule))]
    public class EntityFrameworkCommonModule : Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EntityFrameworkCommonModule).GetAssembly());
        }
    }
}
