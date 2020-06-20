using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.BulkInsert
{
    public class ProviderFactory
    {
        private static Dictionary<string, Func<IEfBulkInsertProvider>> _providers;

        private static Dictionary<string,Func<IEfBulkInsertProvider>> Providers
        {
            get
            {
                if (_providers == null)
                {
                    _providers = new Dictionary<string, Func<IEfBulkInsertProvider>>();
                }
                return _providers;
            }
        }

        public static void Register<T>(string name) where T:IEfBulkInsertProvider,new()
        {
            Providers[name] = (() => (default(T) == null) ? Activator.CreateInstance<T>() : default(T));
        }

        public static IEfBulkInsertProvider Get(DbContext context)
        {
            string connectionTypeName = context.Database.GetDbConnection().GetType().FullName;
            //if (!Providers.ContainsKey(connectionTypeName))
            //{ }

            return Providers[connectionTypeName]().SetContext(context);
        }
    }
}
