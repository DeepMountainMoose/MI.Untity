using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.EF.Core.MySql.Test
{
    public partial class MIContext : DbContextBase
    {
        public MIContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //public DbSet<SlideShowImg> SlideShowImg { get; set; }
    }
}
