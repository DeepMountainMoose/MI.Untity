using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MI.Core.PlugIns
{
    public class PlugInSourceList : List<IPlugInSource>
    {
        /// <summary>
        ///     获取所有模块
        /// </summary>
        /// <returns></returns>
        public List<Type> GetAllModules()
        {
            return this
                .SelectMany(pluginSource => pluginSource.GetModulesWithAllDependencies())
                .Distinct()
                .ToList();
        }
    }
}
