using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.AspNetCore.AspNetCore.Configuration
{
    public class ControllerAssemblySetting
    {
        /// <summary>
        /// "app".
        /// </summary>
        public const string DefaultServiceModuleName = "app";

        public string ModuleName { get; }

        public Assembly Assembly { get; }

        public Func<Type, bool> TypePredicate { get; set; }

        public Action<ControllerModel> ControllerModelConfigurer { get; set; }

        public ControllerAssemblySetting(string moduleName, Assembly assembly)
        {
            ModuleName = moduleName;
            Assembly = assembly;

            TypePredicate = type => true;
            ControllerModelConfigurer = controller => { };
        }
    }
}
