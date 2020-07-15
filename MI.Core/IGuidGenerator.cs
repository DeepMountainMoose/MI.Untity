using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core
{
    /// <summary>
    ///     用于产生Id
    /// </summary>
    public interface IGuidGenerator
    {
        /// <summary>
        ///     产生一个GUID
        /// </summary>
        Guid Create();
    }
}
