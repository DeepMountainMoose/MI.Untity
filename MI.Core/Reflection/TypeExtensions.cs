using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.Core.Reflection
{
    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }
    }
}
