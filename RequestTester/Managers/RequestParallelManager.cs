using RequestTester.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RequestTester.Managers
{
    public static class RequestParallelManager
    {
        public static async Task QueryParallel(RequestCase requestCase, string[] servers, CancellationToken cancellationToken)
        {
            requestCase.isRunning = true;

            var tasks = new Task<Response>[servers.Length];
            for(int i = 0; i< servers.Length; i++)
            {
                tasks[i] = RequestManager.MakeRequest(requestCase.request, servers[i], cancellationToken);
            };

            var results = await Task.WhenAll(tasks);

            requestCase.Responses.Clear();
            for (int i = 0; i < servers.Length; i++)
            {
                requestCase.Responses.Add(servers[i], results[i]);
            }

            requestCase.CompareResults();
            requestCase.isRunning = false;
        }
    }
}
