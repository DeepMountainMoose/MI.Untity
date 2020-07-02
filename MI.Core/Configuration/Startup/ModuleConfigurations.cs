namespace MI.Core.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public ModuleConfigurations(IStartupConfiguration abpConfiguration)
        {
            Configuration = abpConfiguration;
        }

        public IStartupConfiguration Configuration { get; }
    }
}
