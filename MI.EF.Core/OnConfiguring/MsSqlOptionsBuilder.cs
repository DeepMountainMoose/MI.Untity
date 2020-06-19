using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.OnConfiguring
{
    /// <summary>
    /// 配置数据库连接字符串
    /// </summary>
    public class MsSqlOptionsBuilder : OptionsBuilder
    {
        public override DbContextOptionsBuilder GetOptionsBuilder(DbContextOptionsBuilder optionsBuilder, string nameOrConnectionString)
        {
            return optionsBuilder.UseSqlServer(nameOrConnectionString);
        }
    }
}
