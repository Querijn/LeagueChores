using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueChores.Util
{
	internal static class HttpClientExtensions
	{
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            var response = await client.SendAsync(request);
            return response;
        }
    }
}
