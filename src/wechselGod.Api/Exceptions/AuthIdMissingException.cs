using System;
using System.Runtime.Serialization;

namespace wechselGod.Api.Exceptions
{
    public class AuthIdMissingException : Exception
    {
        public AuthIdMissingException()
        {
        }

        public AuthIdMissingException(string message) : base(message)
        {
        }

        public AuthIdMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuthIdMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
