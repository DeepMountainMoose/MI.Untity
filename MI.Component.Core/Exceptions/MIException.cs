using System;
using System.Runtime.Serialization;

namespace MI.Component.Core.Exceptions
{
    public abstract class MIException : Exception
    {
        private const string ErrorCodeField = "ErrorCode";

        public long ErrorCode { get; set; }

        public bool IsLogged { get; set; }

        protected MIException()
        {

        }

        protected MIException(long errorCode, string message) : base(message)
        {
            this.ErrorCode = errorCode;
        }

        protected MIException(long errorCode, Exception innerException) : base(innerException.Message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        protected MIException(long errorCode, Exception innerException, string message) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        protected MIException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.ErrorCode = info.GetInt32("ErrorCode");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ErrorCode", this.ErrorCode);
            base.GetObjectData(info, context);
        }
    }
}
