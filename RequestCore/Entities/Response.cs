using System;
using System.Net;

namespace RequestTester.Entities
{
    public class Response
    {
        //Responce present
        public bool isSuccessed;

        //Responce HTTP code
        public HttpStatusCode statusCode;

        //Exception if any
        public Exception error;

        //Responce body
        public string body;


        public Response(Exception e)
        {
            this.error = e;
        }

        public Response(HttpStatusCode statusCode, string body)
        {
            this.isSuccessed = true;
            this.statusCode = statusCode;
            this.body = body;
        }
    }
}
