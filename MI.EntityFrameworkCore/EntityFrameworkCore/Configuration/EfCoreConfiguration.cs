using Castle.MicroKernel.Registration;
using MI.Core.Dependency;
using Microsoft.EntityFrameworkCore;
using System;

namespace MI.EntityFrameworkCore.EntityFrameworkCore.Configuration
{
    public class EfCoreConfiguration : IEfCoreConfiguration
    {
        private readonly IIocManager _iocManager;

        public EfCoreConfiguration(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        public void AddDbContext<TDbContext>(Action<DbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext
        {
            _iocManager.GetContainer().Register(
                Component.For<IDbContextConfigurer<TDbContext>>().Instance(
                    new DbContextConfigurerAction<TDbContext>(action)
                ).IsDefault()
            );
        }
    }
}
