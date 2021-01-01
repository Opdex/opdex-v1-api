using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules;

namespace Opdex.Core.Infrastructure
{
    public static class CoreInfrastructureServicesExtension
    {
        /// <summary>
        /// Where all Core Infrastructure services are registered.
        /// </summary>
        /// <param name="services">IServiceCollection this method extends on.</param>
        public static void AddCoreInfrastructureServices(this IServiceCollection services)
        {
            AddDataServices(services);
            AddClientServices(services);
        }
        
        private static void AddDataServices(IServiceCollection services)
        {

        }

        private static void AddClientServices(IServiceCollection services)
        {
            #region Cirrus Full Node API
            
            services.AddHttpClient<ISmartContractsModule, SmartContractsModule>(client => client.BuildCirrusHttpClient())
                .AddPolicyHandler(HttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(HttpClientBuilder.GetCircuitBreakerPolicy());

            services.AddHttpClient<IBlockStoreModule, BlockStoreModule>(client => client.BuildCirrusHttpClient())
                .AddPolicyHandler(HttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(HttpClientBuilder.GetCircuitBreakerPolicy());
            
            #endregion
        }
    }
}