using MI.Component.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public class DbContextException : MIException
    {
        public DbContextException(int errorCode, string message)
      : base((long)errorCode, message)
        { }

        public DbContextException(int errorCode, Exception innerException, string message)
            : base((long)errorCode, innerException, message)
        { }
    }
}
