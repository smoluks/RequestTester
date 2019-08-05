using RequestCore.Enums;
using RequestTester.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RequestTester.Managers
{
    public static class RequestParallelManager
    {
        /// <summary>
        /// Send request to some servers parallel
        /// </summary>
        /// <param name="requestCase"></param>
        /// <param name="servers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task SendRequestsParallelAsync(RequestCase requestCase, string[] servers, CancellationToken cancellationToken)
        {
            requestCase._status = CaseStatus.Running;
            requestCase.Responses.Clear();

            var tasks = new Task<Response>[servers.Length];
            for(int i = 0; i< servers.Length; i++)
            {
                tasks[i] = RequestManager.SendRequestAsync(requestCase.request, servers[i], cancellationToken);
            };

            try
            {
                var results = await Task.WhenAll(tasks);                
                for (int i = 0; i < servers.Length; i++)
                {
                    requestCase.Responses.Add(servers[i], results[i]);
                }

                requestCase.CompareResults(cancellationToken);
            }
            catch(TaskCanceledException)
            {
                //cancellationToken in cancelled state
            }
        }
    }
}
