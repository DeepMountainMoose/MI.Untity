using MI.Core.Modules;
using MI.EntityFrameworkCore.EntityFrameworkCore;
using System.Reflection;
using Module = MI.Core.Modules.Module;

namespace MI.Test.Repositories
{
    [DependsOn(typeof(EntityFrameworkCoreModule))]
    public class RepositiesModule : Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(GetType().GetTypeInfo().Assembly);
        }
    }
}
