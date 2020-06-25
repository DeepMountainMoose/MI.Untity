using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.AspNetCore.AspNetCore.Configuration
{
    public interface IAspNetCoreConfiguration
    {
        ControllerAssemblySettingBuilder CreateControllersForAppServices(
            Assembly assembly,
            string moduleName = ControllerAssemblySetting.DefaultServiceModuleName
        );
    }
}
