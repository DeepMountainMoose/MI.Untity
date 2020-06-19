using MI.Component.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.OnConfiguring
{
    public class ObjectContextFactory
    {
        private static Dictionary<string, Func<IOptionsBuilder>> _providers;

        private static Dictionary<string, Func<IOptionsBuilder>> Providers
        {
            get
            {
                if (_providers == null)
                {
                    _providers = new Dictionary<string, Func<IOptionsBuilder>>();
                }

                return _providers;
            }
        }

        public static void Register<T>(string name) where T : IOptionsBuilder, new()
        {
            name = "dbtypeDef";
            Providers[name] = () => (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
        }


        public static IOptionsBuilder Get()
        {
            string connectionTypeName = "dbtypeDef";
            if (!Providers.ContainsKey(connectionTypeName))
            {
                throw new MIParameterException("SqlServer或者Mysql服务未注册！");
            }

            return Providers[connectionTypeName]();
        }
    }
}
