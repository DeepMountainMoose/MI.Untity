using MI.Core.Dependency;
using MI.Core.PlugIns;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core
{
    public class BootstrapperOptions
    {
        public bool DisableAllInterceptors { get; set; }

        public IIocManager IocManager { get; set; }

        public PlugInSourceList PlugInSources { get; }

        public BootstrapperOptions()
        {
            IocManager = Dependency.IocManager.Instance;
            PlugInSources = new PlugInSourceList();
        }
    }
}
