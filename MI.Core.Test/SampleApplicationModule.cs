using MI.Core.Dependency;
using MI.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Core.Test
{
    [DependsOn(typeof(KernelModule))]
    public class SampleApplicationModule : Modules.Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
