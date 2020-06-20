using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public interface IDbInsertTracking: IDbUpdateTracking
    {
        DateTime CreateTime { get; set; }
    }
}
