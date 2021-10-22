using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Opdex.Platform.Common.Converters;
using System.Collections.Generic;

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
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new AddressConverter(),
                    new Sha256Converter(),
                    new UInt128Converter(),
                    new UInt256Converter()
                }
            });

            return new HttpRequestMessage(method, uri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
