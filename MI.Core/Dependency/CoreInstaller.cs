using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MI.Core.Configuration.Startup;
using MI.Core.Modules;
using MI.Core.PlugIns;
using MI.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    internal class CoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ITypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton(),
                Component.For<IStartupConfiguration, StartupConfiguration>()
                    .ImplementedBy<StartupConfiguration>()
                    .LifestyleSingleton(),
                 Component.For<IModuleManager, ModuleManager>().ImplementedBy<ModuleManager>().LifestyleSingleton(),
                Component.For<IAssemblyFinder, AssemblyFinder>().ImplementedBy<AssemblyFinder>().LifestyleSingleton(),
                Component.For<IPlugInManager, PlugInManager>().ImplementedBy<PlugInManager>().LifestyleSingleton()
            );
        }
    }
}
