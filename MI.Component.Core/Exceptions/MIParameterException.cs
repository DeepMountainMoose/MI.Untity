using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Component.Core.Exceptions
{
    public class MIParameterException : MIException
    {
        public MIParameterException(string message) : base(0L, message)
        {

        }
    }
}
