using Microsoft.Extensions.Options;
using RaftLabs.Users.Infrastructure.Options;
namespace RaftLabs.Users.Infrastructure
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetWithHeadersAsync(
            this HttpClient client,
            string requestUri,
            IOptions<ApiOptions> options,
           
            IDictionary<string, string> additionalHeaders = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            // Add default headers
            request.Headers.TryAddWithoutValidation(options.Value.APIKey,  options.Value.APIKeyValue);
            //request.Headers.TryAddWithoutValidation("User-Agent", "MyApp-Agent/1.0");
            //request.Headers.TryAddWithoutValidation("Accept", "application/json");

            // Add additional headers if any
            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return await client.SendAsync(request);
        }
    }

}
