using MI.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MI.Core.Reflection
{
    /// <summary>
    /// Default implementation of <see cref="IAssemblyFinder"/>.
    /// </summary>
    internal class AssemblyFinder : IAssemblyFinder
    {
        private readonly IModuleManager _moduleManager;

        public AssemblyFinder(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
            }

            return assemblies.Distinct().ToList();
        }
    }
}
