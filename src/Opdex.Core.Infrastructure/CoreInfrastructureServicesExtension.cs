using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Data;
using Opdex.Core.Infrastructure.Data.Handlers;

namespace Opdex.Core.Infrastructure
{
    public static class CoreInfrastructureServicesExtension
    {
        /// <summary>
        /// Where all Core Infrastructure services are registered.
        /// </summary>
        /// <param name="services">IServiceCollection this method extends on.</param>
        /// <param name="cirrusConfiguration"></param>
        public static void AddCoreInfrastructureServices(this IServiceCollection services, CirrusConfiguration cirrusConfiguration)
        {
            AddDataServices(services);
            AddClientServices(services, cirrusConfiguration);
        }
        
        private static void AddDataServices(IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();
            
            services.AddTransient<IRequestHandler<SelectAllPairsWithFilterQuery, IEnumerable<Pair>>, SelectAllPairsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllTokensWithFilterQuery, IEnumerable<Token>>, SelectAllTokensWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();
        }

        private static void AddClientServices(IServiceCollection services, CirrusConfiguration cirrusConfiguration)
        {
            #region Cirrus Full Node API
            
            // Modules
            services.AddHttpClient<ISmartContractsModule, SmartContractsModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
                .AddPolicyHandler(HttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(HttpClientBuilder.GetCircuitBreakerPolicy());

            services.AddHttpClient<IBlockStoreModule, BlockStoreModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
                .AddPolicyHandler(HttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(HttpClientBuilder.GetCircuitBreakerPolicy());
            
            services.AddHttpClient<INodeModule, NodeModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
                .AddPolicyHandler(HttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(HttpClientBuilder.GetCircuitBreakerPolicy());
            
            // Queries and Handlers
            services.AddTransient<IRequestHandler<CallCirrusGetCurrentBlockQuery, BlockReceiptDto>, CallCirrusGetCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetBlockByHashQuery, BlockReceiptDto>, CallCirrusGetBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetTransactionReceiptByHashQuery, TransactionReceipt>, CallCirrusGetTransactionReceiptByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusSearchContractTransactionReceiptsQuery, List<TransactionReceipt>>, CallCirrusSearchContractTransactionReceiptsQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenDetailsByAddressQuery, Token>, CallCirrusGetSrcTokenDetailsByAddressQueryHandler>();

            #endregion
        }
    }
}