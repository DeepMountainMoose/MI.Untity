using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.OnConfiguring
{
    public class EFCoreRegister
    {

        public EFCoreRegister()
        { }

        public static void Use<T>()
        {
            Activator.CreateInstance<T>();
        }
    }
    public class SqlServerEFCore
    {
        public SqlServerEFCore()
        {
            ObjectContextFactory.Register<MsSqlOptionsBuilder>("System.Data.SqlClient");
        }
    }
}
