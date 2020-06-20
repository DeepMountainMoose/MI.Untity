using MI.EF.Core.Env;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.EF.Core.WebTest
{
    public partial class MIContext : DbContextBase
    {
        //public MIContext(DbContextOptions<MIContext> options)
        //    : base(options)
        //{

        //}

        public MIContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}
