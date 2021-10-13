using System;
using System.Runtime.Serialization;

namespace UrlShortener.Business
{
    public class ShortUrlException : Exception
    {
        public ShortUrlException()
        {
        }

        public ShortUrlException(string message) : base(message)
        {
        }

        public ShortUrlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ShortUrlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}