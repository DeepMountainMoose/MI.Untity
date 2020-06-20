using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.DbScope
{
    public class DbRetryScopeConfig
    {
        private const int DefaultRetries = 3;

        public int MaximumRetries { get; private set; }

        public DbRetryScopeConfig()
        {
            this.MaximumRetries = DefaultRetries;
        }

        public DbRetryScopeConfig(int maximumRetries)
        {
            this.MaximumRetries = maximumRetries;
        }
    }
}
