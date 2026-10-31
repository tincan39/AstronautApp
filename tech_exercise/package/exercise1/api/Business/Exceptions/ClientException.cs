using System.Net;

namespace StargateAPI.Business.Exceptions
{
    /// <summary>
    /// An expected error caused by the client's request. Its message is safe to return to the client with the given status code.
    /// </summary>
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
