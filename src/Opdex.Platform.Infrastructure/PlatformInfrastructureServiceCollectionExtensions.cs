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
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
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
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers;
using Opdex.Platform.Infrastructure.Data;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Deployers;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

namespace Opdex.Platform.Infrastructure
{
    public static class PlatformInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformInfrastructureServices(this IServiceCollection services, CirrusConfiguration cirrusConfiguration,
            CoinMarketCapConfiguration cmcConfiguration)
        {
            // Data Services
            AddDataQueries(services);
            AddDataCommands(services);
            
            // Client Services
            AddCirrusServices(services, cirrusConfiguration);
            AddCmcServices(services, cmcConfiguration);
            
            return services;
        }

        private static void AddDataCommands(IServiceCollection services)
        {
            // Markets
            services.AddTransient<IRequestHandler<PersistMarketCommand, long>, PersistMarketCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMarketSnapshotCommand, bool>, PersistMarketSnapshotCommandHandler>();

            // Blocks
            services.AddTransient<IRequestHandler<PersistBlockCommand, bool>, PersistBlockCommandHandler>();
            
            // Pools
            services.AddTransient<IRequestHandler<PersistLiquidityPoolCommand, long>, PersistLiquidityPoolCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMiningPoolCommand, long>, PersistMiningPoolCommandHandler>();
            services.AddTransient<IRequestHandler<PersistLiquidityPoolSnapshotCommand, bool>, PersistLiquidityPoolSnapshotCommandHandler>();
            
            // Tokens
            services.AddTransient<IRequestHandler<PersistTokenCommand, long>, PersistTokenCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTokenSnapshotCommand, bool>, PersistTokenSnapshotCommandHandler>();
            
            // Transactions
            services.AddTransient<IRequestHandler<PersistTransactionCommand, Transaction>, PersistTransactionCommandHandler>(); 
            services.AddTransient<IRequestHandler<PersistTransactionLogCommand, bool>, PersistTransactionLogCommandHandler>();
        }
        
        private static void AddDataQueries(IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();
            
            // Deployer
            services.AddTransient<IRequestHandler<SelectDeployerByAddressQuery, Deployer>, SelectDeployerByAddressQueryHandler>();

            // Market 
            services.AddTransient<IRequestHandler<SelectLatestMarketSnapshotQuery, MarketSnapshot>, SelectLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketByAddressQuery, Market>, SelectMarketByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectActiveMarketSnapshotsByMarketIdQuery, IEnumerable<MarketSnapshot>>, SelectActiveMarketSnapshotsByMarketIdQueryHandler>();

            // Blocks
            services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();

            // Pools
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByAddressQuery, LiquidityPool>, SelectLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllLiquidityPoolsQuery, IEnumerable<LiquidityPool>>, SelectAllLiquidityPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectActiveLiquidityPoolSnapshotsByPoolIdQuery, IEnumerable<LiquidityPoolSnapshot>>, SelectActiveLiquidityPoolSnapshotsByPoolIdQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<SelectTokenByIdQuery, Token>, SelectTokenByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>, SelectAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<SelectActiveTokenSnapshotsByTokenIdQuery, IEnumerable<TokenSnapshot>>, SelectActiveTokenSnapshotsByTokenIdQueryHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, SelectTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>, SelectTransactionsByPoolWithFilterQueryHandler>();
        }
        
        private static void AddCirrusServices(IServiceCollection services, CirrusConfiguration cirrusConfiguration)
        {
            // Modules
            services.AddHttpClient<ISmartContractsModule, SmartContractsModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
                .AddPolicyHandler(CirrusHttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(CirrusHttpClientBuilder.GetCircuitBreakerPolicy());

            services.AddHttpClient<IBlockStoreModule, BlockStoreModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
                .AddPolicyHandler(CirrusHttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(CirrusHttpClientBuilder.GetCircuitBreakerPolicy());
            
            services.AddHttpClient<INodeModule, NodeModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
                .AddPolicyHandler(CirrusHttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(CirrusHttpClientBuilder.GetCircuitBreakerPolicy());
            
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
        }

        private static void AddCmcServices(IServiceCollection services, CoinMarketCapConfiguration cmcConfiguration)
        {
            // Modules
            services.AddHttpClient<IQuotesModule, IQuotesModule>(client => client.BuildHttpClient(cmcConfiguration))
                .AddPolicyHandler(CmcHttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(CmcHttpClientBuilder.GetCircuitBreakerPolicy());
            
            // Queries
            services.AddTransient<IRequestHandler<CallCmcGetStraxQuotePriceQuery, decimal>, CallCmcGetStraxQuotePriceQueryHandler>();
        }
    }
}