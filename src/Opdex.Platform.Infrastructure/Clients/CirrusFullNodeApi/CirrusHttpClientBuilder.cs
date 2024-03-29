using System;
using System.Net.Http;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Polly;
using Polly.Extensions.Http;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi;

public static class CirrusHttpClientBuilder
{
    public static void BuildCirrusHttpClient(this HttpClient client, CirrusConfiguration cirrusConfiguration)
    {
        var uri = cirrusConfiguration.ApiPort > 0
            ? $"{cirrusConfiguration.ApiUrl}:{cirrusConfiguration.ApiPort}/api/"
            : $"{cirrusConfiguration.ApiUrl}/api/";

        client.BaseAddress = new Uri(uri);
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        // Circuit break after 25 failures in a row for 30 seconds
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(10, TimeSpan.FromSeconds(30));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        // Retry 6 times and exponentially back off by 1.25 seconds each retry
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.25, retryAttempt)));
    }
}
