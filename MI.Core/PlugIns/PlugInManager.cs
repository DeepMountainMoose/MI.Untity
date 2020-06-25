using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.PlugIns
{
    public class PlugInManager : IPlugInManager
    {
        public PlugInSourceList PlugInSources { get; }

        public PlugInManager()
        {
            PlugInSources = new PlugInSourceList();
        }
    }
}
