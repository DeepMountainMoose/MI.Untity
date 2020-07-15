using MI.Core.Modules;
using MI.EntityFramework.Common;
using MI.EntityFrameworkCore.EntityFrameworkCore.Configuration;

namespace MI.EntityFrameworkCore.EntityFrameworkCore
{
    /// <summary>
    ///     基于Ef Core的数据访问层模块实现
    /// </summary>
    [DependsOn(typeof(EntityFrameworkCommonModule))]
    public class EntityFrameworkCoreModule : Module
    {
        public override void PreInitialize()
        {
            IocManager.RegisterType<IEfCoreConfiguration, EfCoreConfiguration>();
        }
    }
}
