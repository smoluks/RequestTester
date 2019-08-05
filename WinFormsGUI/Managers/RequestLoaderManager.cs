using RequestTester.Entities;
using System;
using System.Collections.Generic;

namespace RequestTester.Managers
{
    public class RequestLoaderManager
    {
        internal static IEnumerable<RequestCase> LoadRequests()
        {
            var requests = new List<RequestCase>();

            Random rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                requests.Add(new RequestCase(new Request()
                {
                    parameters = new Dictionary<string, string> { { "text" , $"{rnd.Next()}" } }
                }));
            }

            return requests;
        }
    }
}
