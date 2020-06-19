using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.OnConfiguring
{
    public abstract class OptionsBuilder:IOptionsBuilder
    {
        public abstract DbContextOptionsBuilder GetOptionsBuilder(DbContextOptionsBuilder optionsBuilder, string nameOrConnectionString);
    }
}
