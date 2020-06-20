using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.DbFunctions
{
    public static class DbFunctionExtensions
    {
        public static void AddFunctions(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SplitStringItem>().HasNoKey();
            modelBuilder.Entity<SplitStringIntItem>().HasNoKey();
            modelBuilder.Entity<SplitStringLongItem>().HasNoKey();
        }
    }
}
