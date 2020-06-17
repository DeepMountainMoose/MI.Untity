using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public interface IDbContextManager<TDbContext> where TDbContext : DbContextBase
    {

    }
}
