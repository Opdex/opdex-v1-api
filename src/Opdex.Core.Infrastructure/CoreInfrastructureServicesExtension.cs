using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pairs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Data;
using Opdex.Core.Infrastructure.Data.Handlers.Blocks;
using Opdex.Core.Infrastructure.Data.Handlers.Pairs;
using Opdex.Core.Infrastructure.Data.Handlers.Tokens;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents;

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
            
            // Blocks
            services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();

            // Pairs
            services.AddTransient<IRequestHandler<SelectAllPairsWithFilterQuery, IEnumerable<Pair>>, SelectAllPairsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectPairByAddressQuery, Pair>, SelectPairByAddressQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<SelectAllTokensWithFilterQuery, IEnumerable<Token>>, SelectAllTokensWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
            
            // Transactions
            services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<SelectBurnEventsByTransactionIdQuery, IEnumerable<BurnEvent>>, SelectBurnEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMintEventsByTransactionIdQuery, IEnumerable<MintEvent>>, SelectMintEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectSwapEventsByTransactionIdQuery, IEnumerable<SwapEvent>>, SelectSwapEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectSyncEventsByTransactionIdQuery, IEnumerable<SyncEvent>>, SelectSyncEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransferEventsByTransactionIdQuery, IEnumerable<TransferEvent>>, SelectTransferEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectApprovalEventsByTransactionIdQuery, IEnumerable<ApprovalEvent>>, SelectApprovalEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectPairCreatedEventsByTransactionIdQuery, IEnumerable<PairCreatedEvent>>, SelectPairCreatedEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionEventSummariesByTransactionIdQuery, IEnumerable<TransactionEventSummary>>, SelectTransactionEventSummariesByTransactionIdQueryHandler>();
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
            services.AddTransient<IRequestHandler<CallCirrusGetTransactionByHashQuery, Transaction>, CallCirrusGetTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusSearchContractTransactionsQuery, List<Transaction>>, CallCirrusSearchContractTransactionsQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenDetailsByAddressQuery, Token>, CallCirrusGetSrcTokenDetailsByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusCallSmartContractMethodCommand, string>, CallCirrusCallSmartContractMethodCommandHandler>();

            #endregion
        }
    }
}