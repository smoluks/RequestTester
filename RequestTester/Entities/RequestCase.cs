using System.Collections.Generic;

namespace RequestTester.Entities
{
    public class RequestCase
    {
        public Request request;

        public Dictionary<string, Response> Responses = new Dictionary<string, Response>();

        public CompareResult result = CompareResult.NotDefined;

        public bool isRunning = false;

        public string Request { get => request.ToString(); }
        public string Status
        {
            get
            {
                if (isRunning)
                    return "Running";
                else
                {
                    switch (result)
                    {
                        case CompareResult.Equals:
                            return "OK";
                        case CompareResult.NotEquals:
                            return "Different";
                        case CompareResult.Error:
                            return "Error";
                        default:
                            return "";
                    }
                }
            }
        }

        public RequestCase(Request request)
        {
            this.request = request;
        }


        public void CompareResults()
        {
            foreach (var responce in Responses.Values)
            {
                if (!responce.isSuccessed)
                {
                    result = CompareResult.Error;
                    return;
                }
            }

            string lastcontent = null;
            foreach (var responce in Responses.Values)
            {
                if (lastcontent != null)
                {
                    if (!responce.data.Equals(lastcontent))
                    {
                        result = CompareResult.NotEquals;
                        return;
                    }
                }
                lastcontent = responce.data;
            }
            result = CompareResult.Equals;
        }

        public enum CompareResult
        {
            NotDefined,
            Equals,
            NotEquals,
            Error
        }
    }
}
