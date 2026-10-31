using System.Net;

namespace StargateAPI.Business.Exceptions
{
    public class ClientException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ClientException(string message) : this(HttpStatusCode.BadRequest, message) { }

        public ClientException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
