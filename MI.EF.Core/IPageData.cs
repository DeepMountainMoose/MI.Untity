using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public class IPageData<T>
    {
        IEnumerable<T> Data { get; }

        int Total { get; }
    }
}
