using RequestCore.Enums;
using System.Collections.Generic;

namespace RequestTester.Entities
{
    public class Request
    {
        //request path without server & port
        public string path = "";

        //request type
        public RequestType requestType = RequestType.GET;

        //path params (name:value)
        public Dictionary<string, string> parameters;

        //request body
        public string content;        

        //get query path with params
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
