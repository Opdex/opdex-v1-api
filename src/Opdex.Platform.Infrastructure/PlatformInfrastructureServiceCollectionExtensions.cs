using Dapper;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernances;
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
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningGovernances;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Data;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Deployers;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Indexer;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningPools;
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
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Summaries;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;
using Opdex.Platform.Infrastructure.Clients.SignalR.Handlers;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Summaries;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Attributes;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens.Wrapped;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Summaries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Wrapped;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.ProposalCertificates;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using Opdex.Platform.Infrastructure.Abstractions.Feeds;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Transactions;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Handlers;
using Opdex.Platform.Infrastructure.Data.Handlers.Auth;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Wrapped;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Pledges;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.ProposalCertificates;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Proposals;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Votes;
using Opdex.Platform.Infrastructure.Feeds;

namespace Opdex.Platform.Infrastructure;

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
        SqlMapper.AddTypeHandler(new Sha256Handler());

        // Data Services
        AddDataQueries(services);
        AddDataCommands(services);

        // Client Services
        AddCirrusServices(services, cirrusConfiguration);
        AddCoinGeckoServices(services);
        AddCmcServices(services, cmcConfiguration);
        AddFeeds(services);
        AddSignalRServices(services);

        return services;
    }

    private static void AddDataCommands(IServiceCollection services)
    {
        // Markets
        services.AddTransient<IRequestHandler<PersistMarketCommand, ulong>, PersistMarketCommandHandler>();
        services.AddTransient<IRequestHandler<PersistMarketPermissionCommand, ulong>, PersistMarketPermissionCommandHandler>();
        services.AddTransient<IRequestHandler<PersistMarketSnapshotCommand, bool>, PersistMarketSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<PersistMarketRouterCommand, bool>, PersistMarketRouterCommandHandler>();
        services.AddTransient<IRequestHandler<PersistMarketSummaryCommand, ulong>, PersistMarketSummaryCommandHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<PersistBlockCommand, bool>, PersistBlockCommandHandler>();
        services.AddTransient<IRequestHandler<PersistIndexerLockCommand, bool>, PersistIndexerLockCommandHandler>();
        services.AddTransient<IRequestHandler<PersistIndexerUnlockCommand, Unit>, PersistIndexerUnlockCommandHandler>();
        services.AddTransient<IRequestHandler<ExecuteRewindToBlockCommand, bool>, ExecuteRewindToBlockCommandHandler>();

        // Liquidity Pools
        services.AddTransient<IRequestHandler<PersistLiquidityPoolCommand, ulong>, PersistLiquidityPoolCommandHandler>();
        services.AddTransient<IRequestHandler<PersistLiquidityPoolSnapshotCommand, bool>, PersistLiquidityPoolSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<PersistLiquidityPoolSummaryCommand, ulong>, PersistLiquidityPoolSummaryCommandHandler>();
        services.AddTransient<IRequestHandler<ExecuteUpdateMarketSummaryLiquidityPoolCountCommand, bool>, ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler>();

        // Mining Pools
        services.AddTransient<IRequestHandler<PersistMiningPoolCommand, ulong>, PersistMiningPoolCommandHandler>();

        // Tokens
        services.AddTransient<IRequestHandler<PersistTokenCommand, ulong>, PersistTokenCommandHandler>();
        services.AddTransient<IRequestHandler<PersistTokenSnapshotCommand, bool>, PersistTokenSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<PersistTokenSummaryCommand, ulong>, PersistTokenSummaryCommandHandler>();
        services.AddTransient<IRequestHandler<PersistTokenAttributeCommand, bool>, PersistTokenAttributeCommandHandler>();
        services.AddTransient<IRequestHandler<PersistTokenWrappedCommand, ulong>, PersistTokenWrappedCommandHandler>();

        // Transactions
        services.AddTransient<IRequestHandler<PersistTransactionCommand, ulong>, PersistTransactionCommandHandler>();
        services.AddTransient<IRequestHandler<PersistTransactionLogCommand, bool>, PersistTransactionLogCommandHandler>();

        // Token Distribution
        services.AddTransient<IRequestHandler<PersistTokenDistributionCommand, bool>, PersistTokenDistributionCommandHandler>();

        // Mining Governances
        services.AddTransient<IRequestHandler<PersistMiningGovernanceCommand, ulong>, PersistMiningGovernanceCommandHandler>();
        services.AddTransient<IRequestHandler<PersistMiningGovernanceNominationCommand, ulong>, PersistMiningGovernanceNominationCommandHandler>();

        // Deployers
        services.AddTransient<IRequestHandler<PersistDeployerCommand, ulong>, PersistDeployerCommandHandler>();

        // Vault
        services.AddTransient<IRequestHandler<PersistVaultCommand, ulong>, PersistVaultCommandHandler>();
        services.AddTransient<IRequestHandler<PersistVaultCertificateCommand, ulong>, PersistVaultCertificateCommandHandler>();
        services.AddTransient<IRequestHandler<PersistVaultProposalCommand, ulong>, PersistVaultProposalCommandHandler>();
        services.AddTransient<IRequestHandler<PersistVaultProposalPledgeCommand, ulong>, PersistVaultProposalPledgeCommandHandler>();
        services.AddTransient<IRequestHandler<PersistVaultProposalVoteCommand, ulong>, PersistVaultProposalVoteCommandHandler>();
        services.AddTransient<IRequestHandler<PersistVaultProposalCertificateCommand, ulong>, PersistVaultProposalCertificateCommandHandler>();

        // Addresses
        services.AddTransient<IRequestHandler<PersistAddressBalanceCommand, ulong>, PersistAddressBalanceCommandHandler>();
        services.AddTransient<IRequestHandler<PersistAddressMiningCommand, ulong>, PersistAddressMiningCommandHandler>();
        services.AddTransient<IRequestHandler<PersistAddressStakingCommand, ulong>, PersistAddressStakingCommandHandler>();
    }

    private static void AddDataQueries(IServiceCollection services)
    {
        services.AddScoped<IDbContext, DbContext>();

        // Admins
        services.AddTransient<IRequestHandler<SelectAdminByAddressQuery, Admin>, SelectAdminByAddressQueryHandler>();

        // Deployer
        services.AddTransient<IRequestHandler<SelectActiveDeployerQuery, Deployer>, SelectActiveDeployerQueryHandler>();
        services.AddTransient<IRequestHandler<SelectDeployerByAddressQuery, Deployer>, SelectDeployerByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectDeployersByModifiedBlockQuery, IEnumerable<Deployer>>, SelectDeployersByModifiedBlockQueryHandler>();

        // Market
        services.AddTransient<IRequestHandler<SelectMarketsWithFilterQuery, IEnumerable<Market>>, SelectMarketsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketSnapshotWithFilterQuery, MarketSnapshot>, SelectMarketSnapshotWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketByAddressQuery, Market>, SelectMarketByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshot>>, SelectMarketSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketByIdQuery, Market>, SelectMarketByIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketPermissionQuery, MarketPermission>, SelectMarketPermissionQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketPermissionsByUserQuery, IEnumerable<MarketPermissionType>>, SelectMarketPermissionsByUserQueryHandler>();
        services.AddTransient<IRequestHandler<SelectActiveMarketRouterByMarketIdQuery, MarketRouter>, SelectActiveMarketRouterByMarketIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketRouterByAddressQuery, MarketRouter>, SelectMarketRouterByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectAllMarketsQuery, IEnumerable<Market>>, SelectAllMarketsQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketPermissionsByModifiedBlockQuery, IEnumerable<MarketPermission>>, SelectMarketPermissionsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketsByModifiedBlockQuery, IEnumerable<Market>>, SelectMarketsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMarketSummaryByMarketIdQuery, MarketSummary>, SelectMarketSummaryByMarketIdQueryHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<SelectLatestBlockQuery, Block>, SelectLatestBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectBlocksWithFilterQuery, IEnumerable<Block>>, SelectBlocksWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectBlockByHeightQuery, Block>, SelectBlockByHeightQueryHandler>();
        services.AddTransient<IRequestHandler<SelectBlockByMedianTimeQuery, Block>, SelectBlockByMedianTimeQueryHandler>();

        // Liquidity Pools
        services.AddTransient<IRequestHandler<SelectLiquidityPoolByAddressQuery, LiquidityPool>, SelectLiquidityPoolByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>, SelectLiquidityPoolsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshot>>, SelectLiquidityPoolSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery, LiquidityPool>, SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolSummaryByLiquidityPoolIdQuery, LiquidityPoolSummary>, SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolByIdQuery, LiquidityPool>, SelectLiquidityPoolByIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolSnapshotWithFilterQuery, LiquidityPoolSnapshot>, SelectLiquidityPoolSnapshotWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery, IEnumerable<LiquidityPool>>, SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler>();

        // Mining Pools
        services.AddTransient<IRequestHandler<SelectMiningPoolsWithFilterQuery, IEnumerable<MiningPool>>, SelectMiningPoolsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningPoolByIdQuery, MiningPool>, SelectMiningPoolByIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningPoolByLiquidityPoolIdQuery, MiningPool>, SelectMiningPoolByLiquidityPoolIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningPoolByAddressQuery, MiningPool>, SelectMiningPoolByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningPoolsByModifiedBlockQuery, IEnumerable<MiningPool>>, SelectMiningPoolsByModifiedBlockQueryHandler>();

        // Tokens
        services.AddTransient<IRequestHandler<SelectTokenByIdQuery, Token>, SelectTokenByIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenByAddressQuery, Token>, SelectTokenByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokensWithFilterQuery, IEnumerable<Token>>, SelectTokensWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshot>>, SelectTokenSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenSnapshotWithFilterQuery, TokenSnapshot>, SelectTokenSnapshotWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectLatestTokenDistributionByTokenIdQuery, TokenDistribution>, SelectLatestTokenDistributionByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectDistributionsByTokenIdQuery, IEnumerable<TokenDistribution>>, SelectDistributionsByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenSummaryByTokenIdQuery, TokenSummary>, SelectTokenSummaryByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenSummaryByMarketAndTokenIdQuery, TokenSummary>, SelectTokenSummaryByMarketAndTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenAttributesByTokenIdQuery, IEnumerable<TokenAttribute>>, SelectTokenAttributesByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenAttributesByTokenAddressQuery, IEnumerable<TokenAttribute>>, SelectTokenAttributesByTokenAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTokenWrappedByTokenIdQuery, TokenWrapped>, SelectTokenWrappedByTokenIdQueryHandler>();

        // Mining Governances
        services.AddTransient<IRequestHandler<SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery, IEnumerable<MiningGovernanceNomination>>, SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningGovernancesWithFilterQuery, IEnumerable<MiningGovernance>>, SelectMiningGovernancesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningGovernanceByAddressQuery, MiningGovernance>, SelectMiningGovernanceByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningGovernanceByTokenIdQuery, MiningGovernance>, SelectMiningGovernanceByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>, SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningGovernancesByModifiedBlockQuery, IEnumerable<MiningGovernance>>, SelectMiningGovernancesByModifiedBlockQueryHandler>();

        // Transactions
        services.AddTransient<IRequestHandler<SelectTransactionByHashQuery, Transaction>, SelectTransactionByHashQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, SelectTransactionLogsByTransactionIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTransactionsWithFilterQuery, IEnumerable<Transaction>>, SelectTransactionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTransactionsForSnapshotRewindQuery, IEnumerable<Transaction>>, SelectTransactionsForSnapshotRewindQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTransactionForVaultProposalVoteRewindQuery, Transaction>, SelectTransactionForVaultProposalVoteRewindQueryHandler>();
        services.AddTransient<IRequestHandler<SelectTransactionForVaultProposalPledgeRewindQuery, Transaction>, SelectTransactionForVaultProposalPledgeRewindQueryHandler>();

        // Vault
        services.AddTransient<IRequestHandler<SelectVaultCertificatesByVaultIdAndOwnerQuery, IEnumerable<VaultCertificate>>, SelectVaultCertificatesByVaultIdAndOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery, VaultProposalPledge>, SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery, VaultProposalVote>, SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalByVaultIdAndPublicIdQuery, VaultProposal>, SelectVaultProposalByVaultIdAndPublicIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultByAddressQuery, Vault>, SelectVaultByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultByIdQuery, Vault>, SelectVaultByIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalByIdQuery, VaultProposal>, SelectVaultProposalByIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultByTokenIdQuery, Vault>, SelectVaultByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultsByModifiedBlockQuery, IEnumerable<Vault>>, SelectVaultsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultsWithFilterQuery, IEnumerable<Vault>>, SelectVaultsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalsByModifiedBlockQuery, IEnumerable<VaultProposal>>, SelectVaultProposalsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalVotesByModifiedBlockQuery, IEnumerable<VaultProposalVote>>, SelectVaultProposalVotesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalPledgesByModifiedBlockQuery, IEnumerable<VaultProposalPledge>>, SelectVaultProposalPledgesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultCertificatesByModifiedBlockQuery, IEnumerable<VaultCertificate>>, SelectVaultCertificatesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>, SelectVaultCertificatesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalsWithFilterQuery, IEnumerable<VaultProposal>>, SelectVaultProposalsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalPledgesWithFilterQuery, IEnumerable<VaultProposalPledge>>, SelectVaultProposalPledgesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalVotesWithFilterQuery, IEnumerable<VaultProposalVote>>, SelectVaultProposalVotesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalCertificateByProposalIdQuery, VaultProposalCertificate>, SelectVaultProposalCertificateByProposalIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultProposalCertificatesByCertificateIdQuery, IEnumerable<VaultProposalCertificate>>, SelectVaultProposalCertificatesByCertificateIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectVaultCertificateByIdQuery, VaultCertificate>, SelectVaultCertificateByIdQueryHandler>();

        // Addresses
        services.AddTransient<IRequestHandler<SelectAddressBalanceByOwnerAndTokenIdQuery, AddressBalance>, SelectAddressBalanceByOwnerAndTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<SelectAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>, SelectAddressMiningByMiningPoolIdAndOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<SelectAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>, SelectAddressStakingByLiquidityPoolIdAndOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<SelectAddressBalancesWithFilterQuery, IEnumerable<AddressBalance>>, SelectAddressBalancesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningPositionsWithFilterQuery, IEnumerable<AddressMining>>, SelectMiningPositionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectStakingPositionsWithFilterQuery, IEnumerable<AddressStaking>>, SelectStakingPositionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<SelectAddressBalancesByModifiedBlockQuery, IEnumerable<AddressBalance>>, SelectAddressBalancesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectMiningPositionsByModifiedBlockQuery, IEnumerable<AddressMining>>, SelectMiningPositionsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<SelectStakingPositionsByModifiedBlockQuery, IEnumerable<AddressStaking>>, SelectStakingPositionsByModifiedBlockQueryHandler>();

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

        services.AddHttpClient<IWalletModule, WalletModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
            .AddPolicyHandler(CirrusHttpClientBuilder.GetRetryPolicy())
            .AddPolicyHandler(CirrusHttpClientBuilder.GetCircuitBreakerPolicy());

        services.AddHttpClient<ISupportedContractsModule, SupportedContractsModule>(client => client.BuildCirrusHttpClient(cirrusConfiguration))
            .AddPolicyHandler(CirrusHttpClientBuilder.GetRetryPolicy())
            .AddPolicyHandler(CirrusHttpClientBuilder.GetCircuitBreakerPolicy());

        // Queries
        services.AddTransient<IRequestHandler<CallCirrusGetBestBlockReceiptQuery, BlockReceipt>, CallCirrusGetBestBlockReceiptQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetBlockReceiptByHashQuery, BlockReceipt>, CallCirrusGetBlockReceiptByHashQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetTransactionByHashQuery, Transaction>, CallCirrusGetTransactionByHashQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetRawTransactionQuery, RawTransactionDto>, CallCirrusGetRawTransactionQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetStandardTokenContractSummaryQuery, StandardTokenContractSummary>, CallCirrusGetStandardTokenContractSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetInterfluxTokenContractSummaryQuery, InterfluxTokenContractSummary>, CallCirrusGetInterfluxTokenContractSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetMiningPoolRewardPerTokenMiningQuery, UInt256>, CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, UInt256>, CallCirrusGetSrcTokenAllowanceQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetOpdexLiquidityPoolReservesQuery, ReservesReceipt>, CallCirrusGetOpdexLiquidityPoolReservesQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetAmountOutStandardQuoteQuery, UInt256>, CallCirrusGetAmountOutStandardQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetAmountInStandardQuoteQuery, UInt256>, CallCirrusGetAmountInStandardQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetAmountOutMultiHopQuoteQuery, UInt256>, CallCirrusGetAmountOutMultiHopQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetAmountInMultiHopQuoteQuery, UInt256>, CallCirrusGetAmountInMultiHopQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetLiquidityAmountInQuoteQuery, UInt256>, CallCirrusGetLiquidityAmountInQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetBlockHashByHeightQuery, Sha256>, CallCirrusGetBlockHashByHeightQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, UInt256>, CallCirrusGetSrcTokenBalanceQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusTrustedWrappedTokenQuery, bool>, CallCirrusTrustedWrappedTokenQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetMiningGovernanceNominationsSummaryQuery, IEnumerable<MiningGovernanceContractNominationSummary>>, CallCirrusGetMiningGovernanceNominationsSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusLocalCallSmartContractMethodCommand, TransactionQuote>, CallCirrusLocalCallSmartContractMethodCommandHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetAddressCrsBalanceQuery, ulong>, CallCirrusGetAddressCrsBalanceQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetMiningBalanceForAddressQuery, UInt256>, CallCirrusGetMiningBalanceForAddressQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetStakingWeightForAddressQuery, UInt256>, CallCirrusGetStakingWeightForAddressQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetSmartContractPropertyQuery, SmartContractMethodParameter>, CallCirrusGetSmartContractPropertyQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetMarketPermissionAuthorizationQuery, bool>, CallCirrusGetMarketPermissionAuthorizationQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusVerifyMessageQuery, bool>, CallCirrusVerifyMessageQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetVaultProposalSummaryByProposalIdQuery, VaultProposalSummary>, CallCirrusGetVaultProposalSummaryByProposalIdQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery, VaultProposalVoteSummary>, CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery, ulong>, CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetVaultContractCertificateSummaryByOwnerQuery, VaultContractCertificateSummary>, CallCirrusGetVaultContractCertificateSummaryByOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<CallCirrusGetContractCodeQuery, ContractCodeDto>, CallCirrusGetContractCodeQueryHandler>();

        // Commands
        services.AddTransient<IRequestHandler<CallCirrusCallSmartContractMethodCommand, Sha256>, CallCirrusCallSmartContractMethodCommandHandler>();
        services.AddTransient<IRequestHandler<CallCirrusCreateSmartContractCommand, Sha256>, CallCirrusCreateSmartContractCommandHandler>();
    }

    private static void AddCoinGeckoServices(IServiceCollection services)
    {
        services.AddHttpClient<ICoinGeckoClient, CoinGeckoClient>(httpClient => new CoinGeckoClient(httpClient))
            .AddPolicyHandler(CmcHttpClientBuilder.GetRetryPolicy())
            .AddPolicyHandler(CmcHttpClientBuilder.GetCircuitBreakerPolicy());

        // Queries
        services.AddTransient<IRequestHandler<CallCoinGeckoGetStraxLatestPriceQuery, decimal>, CallCoinGeckoGetStraxLatestPriceQueryHandler>();
        services.AddTransient<IRequestHandler<CallCoinGeckoGetStraxHistoricalPriceQuery, decimal>, CallCoinGeckoGetStraxHistoricalPriceQueryHandler>();
    }

    private static void AddCmcServices(IServiceCollection services, CoinMarketCapConfiguration cmcConfiguration)
    {
        // Modules
        services.AddHttpClient<IQuotesModule, QuotesModule>(client => client.BuildHttpClient(cmcConfiguration))
            .AddPolicyHandler(CmcHttpClientBuilder.GetRetryPolicy())
            .AddPolicyHandler(CmcHttpClientBuilder.GetCircuitBreakerPolicy());

        // Queries
        services.AddTransient<IRequestHandler<CallCmcGetStraxLatestQuoteQuery, decimal>, CallCmcGetStraxLatestQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<CallCmcGetStraxHistoricalQuoteQuery, decimal>, CallCmcGetStraxHistoricalQuoteQueryHandler>();
    }

    private static void AddFeeds(IServiceCollection services)
    {
        services.AddTransient<IFiatPriceFeed, FiatPriceFeed>();
    }

    private static void AddSignalRServices(IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<NotifyUserOfBroadcastTransactionCommand, Unit>, NotifyUserOfBroadcastTransactionCommandHandler>();
        services.AddTransient<IRequestHandler<NotifyUserOfMinedTransactionCommand, Unit>, NotifyUserOfMinedTransactionCommandHandler>();
    }
}
