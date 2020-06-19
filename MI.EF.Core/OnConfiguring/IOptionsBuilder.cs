using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.OnConfiguring
{
    public interface IOptionsBuilder
    {
        DbContextOptionsBuilder GetOptionsBuilder(DbContextOptionsBuilder optionsBuilder, string nameOrConnectionString);
    }
}
