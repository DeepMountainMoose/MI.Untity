namespace MI.EF.Core
{
    public interface IDbContextManager<TDbContext> : 
        IDbContextQueryAsyncManager<TDbContext>, 
        IDbContextPageAsyncManager<TDbContext>, 
        IDbContextUpdateAsyncManager
        where TDbContext : DbContextBase
    {
        IDbContextQueryAsyncManager<TDbContext> Primary { get; }
    }
}
