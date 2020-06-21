using MI.EF.Core.OnConfiguring;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Component.EF.MySql.Core
{
    public class MySqlConnectionFactory
    {
        public MySqlConnectionFactory()
        {
            ObjectContextFactory.Register<MySqlOptionsBuilder>("MySql.Data.MySqlClient");
        }
    }
}
