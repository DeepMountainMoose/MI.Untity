using MI.Component.Core;
using MI.Component.Core.Exceptions;
using System;

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
