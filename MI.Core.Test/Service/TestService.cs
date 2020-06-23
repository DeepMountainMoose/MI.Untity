using MI.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Core.Test.Service
{
    public class TestService : ITestService, ITransientDependency
    {
        public string GetTestResult()
        {
            return "This is result";
        }
    }
}
