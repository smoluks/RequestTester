using System.Collections.Generic;

namespace RequestTester.Entities
{
    public class Request
    {
        //server path
        public string path = "";

        public RequestType requestType = RequestType.GET;

        public Dictionary<string, string> parameters;

        public string content;

        public enum RequestType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
