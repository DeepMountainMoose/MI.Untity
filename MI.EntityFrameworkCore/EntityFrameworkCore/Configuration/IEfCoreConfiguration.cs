using Microsoft.EntityFrameworkCore;
using System;
namespace MI.EntityFrameworkCore.EntityFrameworkCore.Configuration
{
    public interface IEfCoreConfiguration
    {
        void AddDbContext<TDbContext>(Action<DbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext;
    }
}
