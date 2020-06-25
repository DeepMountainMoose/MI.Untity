using MI.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MI.Core.PlugIns
{
    public interface IPlugInSource
    {
        List<Type> GetModules();
    }

    public static class PlugInSourceExtensions
    {
        public static List<Type> GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(Module.FindDependedModuleTypesRecursivelyIncludingGivenModule)
                .Distinct()
                .ToList();
        }
    }
}
