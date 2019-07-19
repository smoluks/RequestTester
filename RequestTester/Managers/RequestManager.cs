using RequestTester.Entities;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestTester.Managers
{
    public static class RequestManager
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Response> MakeRequest(Request request, string server, CancellationToken cancellationToken)
        {
           

            //send request
            try
            {
                switch(request.requestType)
                {
                    case Request.RequestType.GET:
                        using (var result = await client.GetAsync(server + request.query, cancellationToken))
                        {
                            return new Response(result.StatusCode, await result.Content.ReadAsStringAsync());
                        }
                    case Request.RequestType.POST:
                        using (var content = new StringContent(request.content, UnicodeEncoding.UTF8, "application/json"))
                        using (var result = await client.PostAsync(server + request.query, content, cancellationToken))
                        {
                            return new Response(result.StatusCode, await result.Content.ReadAsStringAsync());
                        }
                    case Request.RequestType.PUT:
                        using (var content = new StringContent(request.content, UnicodeEncoding.UTF8, "application/json"))
                        using (var result = await client.PutAsync(server + request.query, content, cancellationToken))
                        {
                            return new Response(result.StatusCode, await result.Content.ReadAsStringAsync());
                        }
                    case Request.RequestType.DELETE:
                        using (var result = await client.DeleteAsync(server + request.query, cancellationToken))
                        {
                            return new Response(result.StatusCode, await result.Content.ReadAsStringAsync());
                        }
                    default:
                        throw new ApplicationException($"Unknown request type: {request.requestType}");
                }
                
            }
            catch(HttpRequestException e)
            {
                return new Response(e);
            }         
        }
    }
}
