using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opdex.Platform.Common;

namespace Opdex.Platform.Infrastructure.Http;

public abstract class ApiClientBase
{
    protected readonly ILogger _logger;
    protected readonly JsonSerializerSettings _serializerSettings;
    private readonly HttpClient _httpClient;

    protected ApiClientBase(HttpClient httpClient, ILogger logger, JsonSerializerSettings serializerSettings = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serializerSettings = serializerSettings ?? Serialization.DefaultJsonSettings;
    }

    protected Task<TReturn> GetAsync<TReturn>(string uri, CancellationToken cancellationToken)
    {
        return ExecuteCallAsync<TReturn>(() => _httpClient.GetAsync(uri, cancellationToken), uri);
    }

    protected Task<TReturn> PostAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken)
    {
        return ExecuteCallAsync<TReturn>(() => _httpClient.PostAsync(uri, httpContent, cancellationToken), uri);
    }

    protected Task<TReturn> PutAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken)
    {
        return ExecuteCallAsync<TReturn>(() => _httpClient.PutAsync(uri, httpContent, cancellationToken), uri);
    }

    protected Task<TReturn> PatchAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken)
    {
        return ExecuteCallAsync<TReturn>(() => _httpClient.PatchAsync(uri, httpContent, cancellationToken), uri);
    }

    protected Task<TReturn> DeleteAsync<TReturn>(string uri, CancellationToken cancellationToken)
    {
        return ExecuteCallAsync<TReturn>(() => _httpClient.DeleteAsync(uri, cancellationToken), uri);
    }

    private async Task<TReturn> ExecuteCallAsync<TReturn>(Func<Task<HttpResponseMessage>> call, string uri)
    {
        try
        {
            var response = await call();
            return await HandleClientResponse<TReturn>(response);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, $"The request to {uri} failed due to a timeout with {ex.Message}");
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"The request to {uri} failed with {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to parse response from {uri} with {ex.Message}");
            throw;
        }
    }

    private async Task<TReturn> HandleClientResponse<TReturn>(HttpResponseMessage httpResponse)
    {
        httpResponse.EnsureSuccessStatusCode();

        if (httpResponse.StatusCode != HttpStatusCode.NoContent)
        {
            var jsonString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TReturn>(jsonString, _serializerSettings);
        }

        TReturn result = default;
        var returnType = typeof(TReturn);

        return returnType == typeof(bool)
            ? (TReturn)Convert.ChangeType(true, returnType)
            : result;
    }
}