using RequestTester.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RequestTester.Managers
{
    public static class ComparerManager
    {
        public static async Task<(CompareResult result, string error)> Compare(Request request, string[] servers, CancellationToken cancellationToken)
        {
            var tasks = new Task<Response>[servers.Length];
            for(int i = 0; i< servers.Length; i++)
            {
                tasks[i] = RequestManager.MakeRequest(request, servers[i], cancellationToken);
            };

            var results = await Task.WhenAll(tasks);

            string lastcontent = null;
            foreach(var result in results)
            {
                if (!result.isSuccessed)
                    return (CompareResult.Error, result.error);

                if(lastcontent != null)
                {
                    if (!result.data.Equals(lastcontent))
                        return (CompareResult.NotEquals, null);
                }
            }

            return (CompareResult.Equals, null);
        }

        public enum CompareResult
        {
            Equals,
            NotEquals,
            Error
        }
    }
}
