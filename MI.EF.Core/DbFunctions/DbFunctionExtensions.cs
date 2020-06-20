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
            modelBuilder.Query<SplitStringItem>();
            modelBuilder.Query<SplitStringIntItem>();
            modelBuilder.Query<SplitStringLongItem>();
        }
    }
}
