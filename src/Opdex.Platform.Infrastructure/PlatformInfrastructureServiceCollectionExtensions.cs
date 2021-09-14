using Dapper;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Admins;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Governances;
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
using Opdex.Platform.Infrastructure.Data.Handlers.Governances;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Indexer;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Summaries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.SqlMappers;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Balances;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.LiquidityQuotes;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.SwapQuotes;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningPools;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Admins;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Summaries;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;

namespace Opdex.Platform.Infrastructure
{
    public static class PlatformInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformInfrastructureServices(this IServiceCollection services, CirrusConfiguration cirrusConfiguration,
            CoinMarketCapConfiguration cmcConfiguration)
        {
            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new UInt128Handler());
            SqlMapper.AddTypeHandler(new UInt256Handler());
            SqlMapper.AddTypeHandler(new AddressHandler());
            SqlMapper.AddTypeHandler(new FixedDecimalHandler());

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
            services.AddTransient<IRequestHandler<PersistMarketPermissionCommand, long>, PersistMarketPermissionCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMarketSnapshotCommand, bool>, PersistMarketSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMarketRouterCommand, bool>, PersistMarketRouterCommandHandler>();

            // Blocks
            services.AddTransient<IRequestHandler<PersistBlockCommand, bool>, PersistBlockCommandHandler>();
            services.AddTransient<IRequestHandler<PersistIndexerLockCommand, bool>, PersistIndexerLockCommandHandler>();
            services.AddTransient<IRequestHandler<PersistIndexerUnlockCommand, bool>, PersistIndexerUnlockCommandHandler>();
            services.AddTransient<IRequestHandler<ExecuteRewindToBlockCommand, bool>, ExecuteRewindToBlockCommandHandler>();

            // Liquidity Pools
            services.AddTransient<IRequestHandler<PersistLiquidityPoolCommand, long>, PersistLiquidityPoolCommandHandler>();
            services.AddTransient<IRequestHandler<PersistLiquidityPoolSnapshotCommand, bool>, PersistLiquidityPoolSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<PersistLiquidityPoolSummaryCommand, long>, PersistLiquidityPoolSummaryCommandHandler>();

            // Mining Pools
            services.AddTransient<IRequestHandler<PersistMiningPoolCommand, long>, PersistMiningPoolCommandHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<PersistTokenCommand, long>, PersistTokenCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTokenSnapshotCommand, bool>, PersistTokenSnapshotCommandHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<PersistTransactionCommand, long>, PersistTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionLogCommand, bool>, PersistTransactionLogCommandHandler>();

            // Token Distribution
            services.AddTransient<IRequestHandler<PersistTokenDistributionCommand, bool>, PersistTokenDistributionCommandHandler>();

            // Governances
            services.AddTransient<IRequestHandler<PersistMiningGovernanceCommand, long>, PersistMiningGovernanceCommandHandler>();
            services.AddTransient<IRequestHandler<PersistMiningGovernanceNominationCommand, long>, PersistMiningGovernanceNominationCommandHandler>();

            // Deployers
            services.AddTransient<IRequestHandler<PersistDeployerCommand, long>, PersistDeployerCommandHandler>();

            // Vault
            services.AddTransient<IRequestHandler<PersistVaultCommand, long>, PersistVaultCommandHandler>();
            services.AddTransient<IRequestHandler<PersistVaultCertificateCommand, bool>, PersistVaultCertificateCommandHandler>();

            // Addresses
            services.AddTransient<IRequestHandler<PersistAddressBalanceCommand, long>, PersistAddressBalanceCommandHandler>();
            services.AddTransient<IRequestHandler<PersistAddressMiningCommand, long>, PersistAddressMiningCommandHandler>();
            services.AddTransient<IRequestHandler<PersistAddressStakingCommand, long>, PersistAddressStakingCommandHandler>();
        }

        private static void AddDataQueries(IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();

            // Admins
            services.AddTransient<IRequestHandler<SelectAdminByAddressQuery, Admin>, SelectAdminByAddressQueryHandler>();

            // Deployer
            services.AddTransient<IRequestHandler<SelectActiveDeployerQuery, Deployer>, SelectActiveDeployerQueryHandler>();
            services.AddTransient<IRequestHandler<SelectDeployerByAddressQuery, Deployer>, SelectDeployerByAddressQueryHandler>();

            // Market
            services.AddTransient<IRequestHandler<SelectMarketSnapshotWithFilterQuery, MarketSnapshot>, SelectMarketSnapshotWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketByAddressQuery, Market>, SelectMarketByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshot>>, SelectMarketSnapshotsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketByIdQuery, Market>, SelectMarketByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketPermissionQuery, MarketPermission>, SelectMarketPermissionQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketPermissionsByUserQuery, IEnumerable<Permissions>>, SelectMarketPermissionsByUserQueryHandler>();
            services.AddTransient<IRequestHandler<SelectActiveMarketRouterByMarketIdQuery, MarketRouter>, SelectActiveMarketRouterByMarketIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMarketRouterByAddressQuery, MarketRouter>, SelectMarketRouterByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllMarketsQuery, IEnumerable<Market>>, SelectAllMarketsQueryHandler>();

            // Blocks
            services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();
            services.AddTransient<IRequestHandler<SelectBlockByHeightQuery, Block>, SelectBlockByHeightQueryHandler>();

            // Liquidity Pools
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByAddressQuery, LiquidityPool>, SelectLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>, SelectLiquidityPoolsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshot>>, SelectLiquidityPoolSnapshotsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery, LiquidityPool>, SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolSummaryByLiquidityPoolIdQuery, LiquidityPoolSummary>, SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByIdQuery, LiquidityPool>, SelectLiquidityPoolByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolSnapshotWithFilterQuery, LiquidityPoolSnapshot>, SelectLiquidityPoolSnapshotWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLiquidityPoolByLpTokenIdQuery, LiquidityPool>, SelectLiquidityPoolByLpTokenIdQueryHandler>();

            // Mining Pools
            services.AddTransient<IRequestHandler<SelectMiningPoolsWithFilterQuery, IEnumerable<MiningPool>>, SelectMiningPoolsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningPoolByIdQuery, MiningPool>, SelectMiningPoolByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningPoolByLiquidityPoolIdQuery, MiningPool>, SelectMiningPoolByLiquidityPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningPoolByAddressQuery, MiningPool>, SelectMiningPoolByAddressQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<SelectTokenByIdQuery, Token>, SelectTokenByIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokensWithFilterQuery, IEnumerable<Token>>, SelectTokensWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshot>>, SelectTokenSnapshotsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTokenSnapshotWithFilterQuery, TokenSnapshot>, SelectTokenSnapshotWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLatestTokenDistributionQuery, TokenDistribution>, SelectLatestTokenDistributionQueryHandler>();

            // Governances
            services.AddTransient<IRequestHandler<SelectActiveMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceNomination>>, SelectActiveMiningGovernanceNominationsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningGovernancesWithFilterQuery, IEnumerable<MiningGovernance>>, SelectMiningGovernancesWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningGovernanceByAddressQuery, MiningGovernance>, SelectMiningGovernanceByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningGovernanceByTokenIdQuery, MiningGovernance>, SelectMiningGovernanceByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>, SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, SelectTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionsWithFilterQuery, IEnumerable<Transaction>>, SelectTransactionsWithFilterQueryHandler>();

            // Vault
            services.AddTransient<IRequestHandler<SelectVaultsWithFilterQuery, IEnumerable<Vault>>, SelectVaultsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectVaultByAddressQuery, Vault>, SelectVaultByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectVaultByTokenIdQuery, Vault>, SelectVaultByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectVaultCertificatesByOwnerAddressQuery, IEnumerable<VaultCertificate>>, SelectVaultCertificatesByOwnerAddressQueryHandler>();
            services.AddTransient<IRequestHandler<SelectVaultCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>, SelectVaultCertificatesWithFilterQueryHandler>();

            // Addresses
            services.AddTransient<IRequestHandler<SelectAddressBalanceByOwnerAndTokenIdQuery, AddressBalance>, SelectAddressBalanceByOwnerAndTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>, SelectAddressMiningByMiningPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>, SelectAddressStakingByLiquidityPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressBalancesWithFilterQuery, IEnumerable<AddressBalance>>, SelectAddressBalancesWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectMiningPositionsWithFilterQuery, IEnumerable<AddressMining>>, SelectMiningPositionsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectStakingPositionsWithFilterQuery, IEnumerable<AddressStaking>>, SelectStakingPositionsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAddressBalancesByModifiedBlockQuery, IEnumerable<AddressBalance>>, SelectAddressBalancesByModifiedBlockQueryHandler>();

            // Indexer
            services.AddTransient<IRequestHandler<SelectIndexerLockQuery, IndexLock>, SelectIndexerLockQueryHandler>();
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
            services.AddTransient<IRequestHandler<CallCirrusGetCurrentBlockQuery, BlockReceipt>, CallCirrusGetCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetBlockByHashQuery, BlockReceipt>, CallCirrusGetBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetTransactionByHashQuery, Transaction>, CallCirrusGetTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusSearchContractTransactionsQuery, List<Transaction>>, CallCirrusSearchContractTransactionsQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenSummaryByAddressQuery, TokenContractSummary>, CallCirrusGetSrcTokenSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexLiquidityPoolByAddressQuery, LiquidityPool>, CallCirrusGetOpdexLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexMiningPoolByAddressQuery, MiningPoolSmartContractSummary>, CallCirrusGetOpdexMiningPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, UInt256>, CallCirrusGetSrcTokenAllowanceQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetOpdexLiquidityPoolReservesQuery, UInt256[]>, CallCirrusGetOpdexLiquidityPoolReservesQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountOutStandardQuoteQuery, UInt256>, CallCirrusGetAmountOutStandardQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountInStandardQuoteQuery, UInt256>, CallCirrusGetAmountInStandardQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountOutMultiHopQuoteQuery, UInt256>, CallCirrusGetAmountOutMultiHopQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAmountInMultiHopQuoteQuery, UInt256>, CallCirrusGetAmountInMultiHopQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAddLiquidityQuoteQuery, UInt256>, CallCirrusGetAddLiquidityQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetStakingTokenSummaryByAddressQuery, StakingTokenContractSummary>, CallCirrusGetStakingTokenSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetMiningGovernanceSummaryByAddressQuery, MiningGovernanceContractSummary>, CallCirrusGetMiningGovernanceSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetBlockHashByHeightQuery, string>, CallCirrusGetBlockHashByHeightQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, UInt256>, CallCirrusGetSrcTokenBalanceQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetMiningGovernanceSummaryNominationsQuery, IEnumerable<MiningGovernanceNominationCirrusDto>>, CallCirrusGetMiningGovernanceSummaryNominationsQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetVaultTotalSupplyQuery, UInt256>, CallCirrusGetVaultTotalSupplyQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusLocalCallSmartContractMethodCommand, TransactionQuote>, CallCirrusLocalCallSmartContractMethodCommandHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetAddressBalanceQuery, ulong>, CallCirrusGetAddressBalanceQueryHandler>();
            services.AddTransient<IRequestHandler<CallCirrusGetMiningPoolByTokenQuery, Address>, CallCirrusGetMiningPoolByTokenQueryHandler>();

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
