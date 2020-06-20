namespace MI.EF.Core.Env
{
    public interface IEnvironmentHandler<TDbContext>
        where TDbContext : DbContextBase
    {
        IDbContextManager<TDbContext> Db { get; }
    }
}