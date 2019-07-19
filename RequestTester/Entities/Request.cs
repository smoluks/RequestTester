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

        public string query
        {
            get
            {
                string requestString = path;
                if (parameters != null && parameters.Count > 0)
                {
                    requestString += "?";
                    foreach (var parm in parameters)
                    {
                        requestString += $"{parm.Key}={parm.Value}&";
                    }
                }

                return requestString;
            }
        }

        public override string ToString()
        {
            return $"{requestType.ToString()}: {query}";
        }
    }
}
