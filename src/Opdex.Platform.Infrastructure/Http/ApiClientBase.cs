using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Http;

public abstract class ApiClientBase
{
    protected readonly ILogger<ApiClientBase> _logger;
    protected readonly JsonSerializerSettings _serializerSettings;
    private readonly HttpClient _httpClient;

    protected ApiClientBase(HttpClient httpClient, ILogger<ApiClientBase> logger, JsonSerializerSettings serializerSettings = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializerSettings = serializerSettings ?? Serialization.DefaultJsonSettings;
    }

    protected async Task<TReturn> GetAsync<TReturn>(string uri, bool findOrThrow = true, CancellationToken cancellationToken = default)
    {
        return await ExecuteCallAsync<TReturn>(() => _httpClient.GetAsync(uri, cancellationToken), uri, findOrThrow, cancellationToken);
    }

    protected async Task<TReturn> PostAsync<TReturn>(string uri, HttpContent httpContent, bool findOrThrow = true, CancellationToken cancellationToken = default)
    {
        return await ExecuteCallAsync<TReturn>(() => _httpClient.PostAsync(uri, httpContent, cancellationToken), uri, findOrThrow, cancellationToken);
    }

    protected async Task<TReturn> PutAsync<TReturn>(string uri, HttpContent httpContent, bool findOrThrow = true, CancellationToken cancellationToken = default)
    {
        return await ExecuteCallAsync<TReturn>(() => _httpClient.PutAsync(uri, httpContent, cancellationToken), uri, findOrThrow, cancellationToken);
    }

    protected async Task<TReturn> PatchAsync<TReturn>(string uri, HttpContent httpContent, bool findOrThrow = true, CancellationToken cancellationToken = default)
    {
        return await ExecuteCallAsync<TReturn>(() => _httpClient.PatchAsync(uri, httpContent, cancellationToken), uri, findOrThrow, cancellationToken);
    }

    protected async Task<TReturn> DeleteAsync<TReturn>(string uri, bool findOrThrow = true, CancellationToken cancellationToken = default)
    {
        return await ExecuteCallAsync<TReturn>(() => _httpClient.DeleteAsync(uri, cancellationToken), uri, findOrThrow, cancellationToken);
    }

    private async Task<TReturn> ExecuteCallAsync<TReturn>(Func<Task<HttpResponseMessage>> call, string uri, bool findOrThrow, CancellationToken cancellationToken)
    {
        try
        {
            var response = await call();
            return await HandleClientResponse<TReturn>(response, findOrThrow, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "The request to {Uri} failed with {Message}", uri, ex.Message);
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "The request to {Uri} failed with {Message}", uri, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The request to {Uri} failed with {Message}", uri, ex.Message);
            throw;
        }
    }

    private async Task<TReturn> HandleClientResponse<TReturn>(HttpResponseMessage httpResponse, bool findOrThrow, CancellationToken cancellationToken)
    {
        if (findOrThrow) httpResponse.EnsureSuccessStatusCode();

        if (httpResponse.StatusCode != HttpStatusCode.NoContent)
        {
            var jsonString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                return JsonConvert.DeserializeObject<TReturn>(jsonString, _serializerSettings);
            }
            catch
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["Response"] = jsonString }))
                {
                    _logger.LogError("Unable to deserialize expected response type");
                }

                throw;
            }
        }

        TReturn result = default;
        var returnType = typeof(TReturn);

        return returnType == typeof(bool)
            ? (TReturn)Convert.ChangeType(true, returnType)
            : result;
    }
}
