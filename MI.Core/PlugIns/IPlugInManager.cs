using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.PlugIns
{
    public interface IPlugInManager
    {
        PlugInSourceList PlugInSources { get; }
    }
}
