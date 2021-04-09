using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Data;
using Opdex.Core.Infrastructure.Data.Handlers.Blocks;
using Opdex.Core.Infrastructure.Data.Handlers.Pools;
using Opdex.Core.Infrastructure.Data.Handlers.Tokens;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionLogs;

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

            // Pools
            services.AddTransient<IRequestHandler<SelectAllPoolsWithFilterQuery, IEnumerable<Pool>>, SelectAllPoolsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectPoolByAddressQuery, Pool>, SelectPoolByAddressQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<SelectAllTokensWithFilterQuery, IEnumerable<Token>>, SelectAllTokensWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
            
            // Transactions
            services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<SelectBurnLogsByIdsQuery, IEnumerable<BurnLog>>, SelectBurnLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMintLogsByIdsQuery, IEnumerable<MintLog>>, SelectMintLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectSwapLogsByIdsQuery, IEnumerable<SwapLog>>, SelectSwapLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectReservesLogsByIdsQuery, IEnumerable<ReservesLog>>, SelectReservesLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransferLogsByIdsQuery, IEnumerable<TransferLog>>, SelectTransferLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectApprovalLogsByIdsQuery, IEnumerable<ApprovalLog>>, SelectApprovalLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolCreatedLogsByIdsQuery, IEnumerable<LiquidityPoolCreatedLog>>, SelectLiquidityPoolCreatedLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionLogSummariesByTransactionIdQuery, IEnumerable<TransactionLogSummary>>, SelectTransactionLogSummariesByTransactionIdQueryHandler>();
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
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexPoolByAddressQuery, Pool>, CallCirrusGetOpdexPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, string>, CallCirrusGetSrcTokenAllowanceQueryHandler>();

            // Commands
            services.AddTransient<IRequestHandler<CallCirrusCallSmartContractMethodCommand, string>, CallCirrusCallSmartContractMethodCommandHandler>();

            #endregion
        }
    }
}