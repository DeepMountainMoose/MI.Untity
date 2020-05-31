using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI
{
    public interface IEntryAssemblyResolver
    {
        Assembly GetEntryAssembly();
    }

    public class DefaultEntryAssemblyResolver : IEntryAssemblyResolver
    {
        public Assembly GetEntryAssembly() => Assembly.GetEntryAssembly();
    }
}
