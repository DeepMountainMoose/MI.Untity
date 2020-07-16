using System;
using System.Linq;

namespace MI.Domain.Test.Interface.EntityFramework
{
    public interface ITestCurrentContext:IDisposable
    {
        IQueryable<SlideShowImg> SlideShowImg { get; }
    }
}
