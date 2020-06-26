using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Exceptions
{
    public class LibraryException : Exception
    {
        public LibraryException(string message) : base(message)
        {

        }
    }
}
