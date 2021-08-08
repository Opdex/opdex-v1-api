using System;
using System.Net.Http;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Polly;
using Polly.Extensions.Http;

namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi
{
    public static class CmcHttpClientBuilder
    {
        public static void BuildHttpClient(this HttpClient client, CoinMarketCapConfiguration cmcConfiguration)
        {
            client.BaseAddress = new Uri(cmcConfiguration.ApiUrl);
            client.DefaultRequestHeaders.Add(cmcConfiguration.HeaderName, cmcConfiguration.ApiKey);
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
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)));
        }
    }
}