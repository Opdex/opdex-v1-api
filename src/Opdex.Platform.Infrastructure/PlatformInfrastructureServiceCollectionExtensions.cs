using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Market;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Data;
using Opdex.Platform.Infrastructure.Data.Handlers;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Market;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.TransactionLogs;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs;

namespace Opdex.Platform.Infrastructure
{
    public static class PlatformInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformInfrastructureServices(this IServiceCollection services, CirrusConfiguration cirrusConfiguration)
        {
            // Data Services
            DataQueries(services);
            
            // Client Services
            AddClientServices(services, cirrusConfiguration);
            
            return services;
        }

        private static void DataCommands(IServiceCollection services)
        {
            // Blocks
            services.AddTransient<IRequestHandler<PersistBlockCommand, bool>, PersistBlockCommandHandler>();
            
            // Pools
            services.AddTransient<IRequestHandler<PersistLiquidityPoolCommand, long>, PersistLiquidityPoolCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMiningPoolCommand, long>, PersistMiningPoolCommandHandler>();
            
            // Tokens
            services.AddTransient<IRequestHandler<PersistTokenCommand, long>, PersistTokenCommandHandler>();
            
            // Transactions
            services.AddTransient<IRequestHandler<PersistTransactionCommand, Transaction>, PersistTransactionCommandHandler>(); 
            services.AddTransient<IRequestHandler<PersistTransactionLogCommand, bool>, PersistTransactionLogCommandHandler>();
        }
        
        private static void DataQueries(IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();
            
            // Market 
            services.AddTransient<IRequestHandler<SelectLatestMarketSnapshotQuery, MarketSnapshot>, SelectLatestMarketSnapshotQueryHandler>();

            // Blocks
            services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();

            // Pools
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByAddressQuery, LiquidityPool>, SelectLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllLiquidityPoolsQuery, IEnumerable<LiquidityPool>>, SelectAllLiquidityPoolsQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<SelectTokenByIdQuery, Token>, SelectTokenByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>, SelectAllTokensQueryHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, SelectTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>, SelectTransactionsByPoolWithFilterQueryHandler>();
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
            
            // Queries
            services.AddTransient<IRequestHandler<CallCirrusGetCurrentBlockQuery, BlockReceiptDto>, CallCirrusGetCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetBlockByHashQuery, BlockReceiptDto>, CallCirrusGetBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetTransactionByHashQuery, Transaction>, CallCirrusGetTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusSearchContractTransactionsQuery, List<Transaction>>, CallCirrusSearchContractTransactionsQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenDetailsByAddressQuery, Token>, CallCirrusGetSrcTokenDetailsByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexLiquidityPoolByAddressQuery, LiquidityPool>, CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexMiningPoolByAddressQuery, MiningPool>, CallCirrusGetOpdexMiningPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, string>, CallCirrusGetSrcTokenAllowanceQueryHandler>();

            // Commands
            services.AddTransient<IRequestHandler<CallCirrusCallSmartContractMethodCommand, string>, CallCirrusCallSmartContractMethodCommandHandler>();

            #endregion
        }
    }
}