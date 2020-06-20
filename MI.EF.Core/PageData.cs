using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public class PageData<T>:IPageData<T>
    {
        public IEnumerable<T> Data { get; private set; }

        public int Total { get; private set; }

        public PageData(IEnumerable<T> Data,int Total)
        {
            this.Data = Data;
            this.Total = Total;
        }
    }
}
