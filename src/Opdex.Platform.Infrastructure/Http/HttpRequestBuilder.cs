using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Opdex.Platform.Infrastructure.Http
{
    // Todo: Move this to HTTP directory
    // Any integration specific configurations, should modify these properties
    public static class HttpRequestBuilder
    {
        public static HttpRequestMessage BuildHttpRequestMessage(object request, string uri, HttpMethod method)
        {
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            return new HttpRequestMessage(method, uri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}