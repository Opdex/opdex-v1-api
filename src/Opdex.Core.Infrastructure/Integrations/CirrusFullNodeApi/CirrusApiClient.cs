using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opdex.Core.Infrastructure.Abstractions.Integrations;

namespace Opdex.BasePlatform.Infrastructure.Integrations.CirrusFullNodeApi
{
    // Todo: Most of this should inherit from a ApiClientBase class
    public class CirrusApiClient : IApiClient
    {
        private static readonly List<HttpStatusCode> SuccessStatusCodes = new List<HttpStatusCode>
        {
            HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted, HttpStatusCode.NoContent
        };

        private readonly ILogger<CirrusApiClient> _logger;
        private readonly HttpClient _httpClient;

        public CirrusApiClient(HttpClient httpClient, ILogger<CirrusApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TReturn> GetAsync<TReturn>(string uri, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(uri, cancellationToken);

            return await HandleClientResponse<TReturn>(response);
        }

        public async Task<TReturn> PostAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync(uri, httpContent, cancellationToken);

            return await HandleClientResponse<TReturn>(response);
        }

        public async Task<TReturn> PutAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsync(uri, httpContent, cancellationToken);

            return await HandleClientResponse<TReturn>(response);
        }

        public async Task<TReturn> PatchAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PatchAsync(uri, httpContent, cancellationToken);

            return await HandleClientResponse<TReturn>(response);
        }

        public async Task<TReturn> DeleteAsync<TReturn>(string uri, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync(uri, cancellationToken);

            return await HandleClientResponse<TReturn>(response);
        }

        private async Task<TReturn> HandleClientResponse<TReturn>(HttpResponseMessage httpResponse)
        {
            try
            {
                var jsonString = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    var returnType = typeof(TReturn);
                    TReturn result = default;

                    if (typeof(bool) == returnType)
                    {
                        result = (TReturn)Convert.ChangeType(true, returnType);
                    }

                    return result;
                }

                if (SuccessStatusCodes.Contains(httpResponse.StatusCode))
                {
                    return JsonConvert.DeserializeObject<TReturn>(jsonString);
                }

                // Todo: return, throw, check status code to act accordingly etc.
                throw new Exception($"Unsuccessful response code of {httpResponse.StatusCode}");
            }
            //catch (CirrusApiException ex)
            //{
            //    _logger.LogError(ex, ex.Message);
            //    throw;
            //}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to parse the response.");
                throw;
            }
        }
    }
}