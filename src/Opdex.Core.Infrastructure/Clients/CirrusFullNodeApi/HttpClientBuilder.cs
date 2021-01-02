using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi
{
    // Todo: Move some of this core logic into Http directory
    // Use Cirrus specific configurations to modify taret params
    public static class HttpClientBuilder
    {
        public static void BuildCirrusHttpClient(this HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:37223/api/");
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            // Circuit break after 25 failures in a row for 30 seconds
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(25, TimeSpan.FromSeconds(30));
        }
        
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Retry 6 times and exponentially back off by 2 seconds each retry
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}