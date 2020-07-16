using System;
using System.Linq;


namespace MI.Domain.Test.Interface.EntityFramework
{
    public interface ITestReadCurrentContext : IDisposable
    {
        IQueryable<SlideShowImg> SlideShowImg { get; }
    }
}
