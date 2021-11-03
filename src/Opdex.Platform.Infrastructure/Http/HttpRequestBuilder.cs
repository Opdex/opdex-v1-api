using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Opdex.Platform.Common;

namespace Opdex.Platform.Infrastructure.Http
{
    // Todo: Move this to HTTP directory
    // Any integration specific configurations, should modify these properties
    public static class HttpRequestBuilder
    {
        public static HttpRequestMessage BuildHttpRequestMessage(object request, string uri, HttpMethod method, JsonSerializerSettings serializerSettings = null)
        {
            var json = JsonConvert.SerializeObject(request, serializerSettings ?? Serialization.DefaultJsonSettings);

            return new HttpRequestMessage(method, uri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
