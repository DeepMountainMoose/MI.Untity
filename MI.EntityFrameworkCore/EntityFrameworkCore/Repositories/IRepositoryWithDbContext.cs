using Microsoft.EntityFrameworkCore;

namespace MI.EntityFrameworkCore.EntityFrameworkCore.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}
