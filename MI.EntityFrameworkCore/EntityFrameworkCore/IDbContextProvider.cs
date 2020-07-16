using Microsoft.EntityFrameworkCore;

namespace MI.EntityFrameworkCore.EntityFrameworkCore
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }
}
