using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Domain;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.LiquidityQuotes;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningGovernance;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools.LiquidityQuotes;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools.SwapQuotes;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Data;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Deployers;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernance;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Vault;

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
            services.AddTransient<IRequestHandler<PersistTransactionCommand, long>, PersistTransactionCommandHandler>(); 
            services.AddTransient<IRequestHandler<PersistTransactionLogCommand, bool>, PersistTransactionLogCommandHandler>();
            
            // Token Distribution
            services.AddTransient<IRequestHandler<PersistTokenDistributionCommand, bool>, PersistTokenDistributionCommandHandler>();
            
            // Mining Governance
            services.AddTransient<IRequestHandler<PersistMiningGovernanceCommand, long>, PersistMiningGovernanceCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMiningGovernanceNominationCommand, long>, PersistMiningGovernanceNominationCommandHandler>();

            // Deployers
            services.AddTransient<IRequestHandler<PersistDeployerCommand, long>, PersistDeployerCommandHandler>();
            
            // Vault
            services.AddTransient<IRequestHandler<PersistVaultCommand, long>, PersistVaultCommandHandler>();
            services.AddTransient<IRequestHandler<PersistVaultCertificateCommand, bool>, PersistVaultCertificateCommandHandler>();
            
            // Addresses
            services.AddTransient<IRequestHandler<PersistAddressBalanceCommand, long>, PersistAddressBalanceCommandHandler>();
            services.AddTransient<IRequestHandler<PersistAddressAllowanceCommand, long>, PersistAddressAllowanceCommandHandler>();
            services.AddTransient<IRequestHandler<PersistAddressMiningCommand, long>, PersistAddressMiningCommandHandler>();
            services.AddTransient<IRequestHandler<PersistAddressStakingCommand, long>, PersistAddressStakingCommandHandler>();
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
            services.AddTransient<IRequestHandler<SelectMarketByIdQuery, Market>, SelectMarketByIdQueryHandler>();

            // Blocks
            services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();
            services.AddTransient<IRequestHandler<SelectBlockByHeightQuery, Block>, SelectBlockByHeightQueryHandler>();

            // Pools
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByAddressQuery, LiquidityPool>, SelectLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllLiquidityPoolsQuery, IEnumerable<LiquidityPool>>, SelectAllLiquidityPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectActiveLiquidityPoolSnapshotsByPoolIdQuery, IEnumerable<LiquidityPoolSnapshot>>, SelectActiveLiquidityPoolSnapshotsByPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByTokenIdAndMarketAddressQuery, LiquidityPool>, SelectLiquidityPoolByTokenIdAndMarketAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningPoolByLiquidityPoolIdQuery, MiningPool>, SelectMiningPoolByLiquidityPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningPoolByAddressQuery, MiningPool>, SelectMiningPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByIdQuery, LiquidityPool>, SelectLiquidityPoolByIdQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<SelectTokenByIdQuery, Token>, SelectTokenByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>, SelectAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery, IEnumerable<TokenSnapshot>>, SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLatestTokenSnapshotByTokenIdQuery, TokenSnapshot>, SelectLatestTokenSnapshotByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLatestTokenDistributionQuery, TokenDistribution>, SelectLatestTokenDistributionQueryHandler>();
            
            // Mining Governance
            services.AddTransient<IRequestHandler<SelectMiningGovernanceByTokenIdQuery, MiningGovernance>, SelectMiningGovernanceByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectActiveMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceNomination>>, SelectActiveMiningGovernanceNominationsQueryHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, SelectTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>, SelectTransactionsByPoolWithFilterQueryHandler>();
            
            // Vault
            services.AddTransient<IRequestHandler<SelectVaultQuery, Vault>, SelectVaultQueryHandler>();
            services.AddTransient<IRequestHandler<SelectVaultCertificatesByOwnerAddressQuery, IEnumerable<VaultCertificate>>, SelectVaultCertificatesByOwnerAddressQueryHandler>();
            
            // Addresses
            services.AddTransient<IRequestHandler<SelectAddressBalanceByLiquidityPoolIdAndOwnerQuery, AddressBalance>, SelectAddressBalanceByLiquidityPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressBalanceByTokenIdAndOwnerQuery, AddressBalance>, SelectAddressBalanceByTokenIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>, SelectAddressMiningByMiningPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>, SelectAddressStakingByLiquidityPoolIdAndOwnerQueryHandler>();
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
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexMiningPoolByAddressQuery, MiningPoolSmartContractSummary>, CallCirrusGetOpdexMiningPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, string>, CallCirrusGetSrcTokenAllowanceQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexLiquidityPoolReservesQuery, string[]>, CallCirrusGetOpdexLiquidityPoolReservesQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountOutStandardQuoteQuery, string>, CallCirrusGetAmountOutStandardQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountInStandardQuoteQuery, string>, CallCirrusGetAmountInStandardQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountOutMultiHopQuoteQuery, string>, CallCirrusGetAmountOutMultiHopQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountInMultiHopQuoteQuery, string>, CallCirrusGetAmountInMultiHopQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAddLiquidityQuoteQuery, string>, CallCirrusGetAddLiquidityQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetStakingTokenSummaryByAddressQuery, StakingTokenContractSummary>, CallCirrusGetStakingTokenSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetMiningGovernanceSummaryByAddressQuery, MiningGovernanceContractSummary>, CallCirrusGetMiningGovernanceSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetBlockHashByHeightQuery, string>, CallCirrusGetBlockHashByHeightQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, string>, CallCirrusGetSrcTokenBalanceQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetMiningGovernanceSummaryNominationsQuery, IEnumerable<MiningGovernanceNominationCirrusDto>>, CallCirrusGetMiningGovernanceSummaryNominationsQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusLocalCallSmartContractMethodQuery, LocalCallResponseDto>, CallCirrusLocalCallSmartContractMethodQueryHandler>();

            // Commands
            services.AddTransient<IRequestHandler<CallCirrusCallSmartContractMethodCommand, string>, CallCirrusCallSmartContractMethodCommandHandler>();
            services.AddTransient<IRequestHandler<CallCirrusCreateSmartContractCommand, string>, CallCirrusCreateSmartContractCommandHandler>();
        }

        private static void AddCmcServices(IServiceCollection services, CoinMarketCapConfiguration cmcConfiguration)
        {
            // Modules
            services.AddHttpClient<IQuotesModule, QuotesModule>(client => client.BuildHttpClient(cmcConfiguration))
                .AddPolicyHandler(CmcHttpClientBuilder.GetRetryPolicy())
                .AddPolicyHandler(CmcHttpClientBuilder.GetCircuitBreakerPolicy());
            
            // Queries
            services.AddTransient<IRequestHandler<CallCmcGetStraxQuotePriceQuery, decimal>, CallCmcGetStraxQuotePriceQueryHandler>();
        }
    }
}