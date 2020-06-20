using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public interface IDbUpdateTracking
    {
        DateTime LastUpdateTime { get; set; }
    }
}
