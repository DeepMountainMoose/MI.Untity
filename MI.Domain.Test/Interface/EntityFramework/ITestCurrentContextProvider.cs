using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Domain.Test.Interface.EntityFramework
{
    public interface ITestCurrentContextProvider
    {
        ITestReadCurrentContext GetReadContext();

        ITestCurrentContext GetContext();
    }
}
