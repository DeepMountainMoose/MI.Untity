using MI.EF.Core.OnConfiguring;
using Microsoft.EntityFrameworkCore;

namespace MI.Component.EF.MySql.Core
{
    public class MySqlOptionsBuilder : OptionsBuilder
    {
        public override DbContextOptionsBuilder GetOptionsBuilder(DbContextOptionsBuilder optionsBuilder, string nameOrConnectionString)
        {
            return optionsBuilder.UseMySql(nameOrConnectionString);
        }
    }
}
