using System.Net;
using System.Net.Http;

namespace RequestTester.Entities
{
    public class Response
    {
        public bool isSuccessed;
        public string error;
        public string data;
        public HttpStatusCode statusCode;

        public Response(HttpRequestException e)
        {
            this.error = e.Message;
        }

        public Response(HttpStatusCode statusCode, string data)
        {
            this.isSuccessed = true;
            this.statusCode = statusCode;
            this.data = data;
        }
    }
}
