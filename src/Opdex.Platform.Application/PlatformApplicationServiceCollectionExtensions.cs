using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Application.Abstractions.Cache;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.Commands.Auth;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.Assemblers.TransactionEvents;
using Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens;
using Opdex.Platform.Application.EntryHandlers;
using Opdex.Platform.Application.EntryHandlers.Addresses.Allowances;
using Opdex.Platform.Application.EntryHandlers.Addresses.Balances;
using Opdex.Platform.Application.EntryHandlers.Addresses.Mining;
using Opdex.Platform.Application.EntryHandlers.Addresses.Staking;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Application.EntryHandlers.Deployers;
using Opdex.Platform.Application.EntryHandlers.MiningGovernances;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Application.EntryHandlers.Markets.Permissions;
using Opdex.Platform.Application.EntryHandlers.Markets.Quotes;
using Opdex.Platform.Application.EntryHandlers.Markets.Snapshots;
using Opdex.Platform.Application.EntryHandlers.MarketTokens;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes;
using Opdex.Platform.Application.EntryHandlers.Routers;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;
using Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernances;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Handlers;
using Opdex.Platform.Application.Handlers.Addresses.Allowances;
using Opdex.Platform.Application.Handlers.Addresses.Balances;
using Opdex.Platform.Application.Handlers.Addresses.Mining;
using Opdex.Platform.Application.Handlers.Addresses.Staking;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Application.Handlers.Deployers;
using Opdex.Platform.Application.Handlers.MiningGovernances;
using Opdex.Platform.Application.Handlers.MiningGovernances.Nominations;
using Opdex.Platform.Application.Handlers.Indexer;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Handlers.Markets;
using Opdex.Platform.Application.Handlers.Markets.Permissions;
using Opdex.Platform.Application.Handlers.Markets.Snapshots;
using Opdex.Platform.Application.Handlers.MiningPools;
using Opdex.Platform.Application.Handlers.Routers;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Application.Handlers.Tokens.Attributes;
using Opdex.Platform.Application.Handlers.Tokens.Distribution;
using Opdex.Platform.Application.Handlers.Tokens.Snapshots;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Application.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Certificates;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Votes;
using Opdex.Platform.Application.Abstractions.Models.Auth;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Index;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Auth;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.ProposalCertificates;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;
using Opdex.Platform.Application.Cache;
using Opdex.Platform.Application.EntryHandlers.Auth;
using Opdex.Platform.Application.EntryHandlers.Indexer;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults.Certificates;
using Opdex.Platform.Application.EntryHandlers.Vaults.Pledges;
using Opdex.Platform.Application.EntryHandlers.Vaults.Proposals;
using Opdex.Platform.Application.EntryHandlers.Vaults.Votes;
using Opdex.Platform.Application.Handlers.Auth;
using Opdex.Platform.Application.Handlers.Markets.Summaries;
using Opdex.Platform.Application.Handlers.Tokens.Wrapped;
using Opdex.Platform.Application.Handlers.Vaults;
using Opdex.Platform.Application.Handlers.Vaults.Certificates;
using Opdex.Platform.Application.Handlers.Vaults.Pledges;
using Opdex.Platform.Application.Handlers.Vaults.ProposalCertificates;
using Opdex.Platform.Application.Handlers.Vaults.Proposals;
using Opdex.Platform.Application.Handlers.Vaults.Votes;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.Application;

public static class PlatformApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
        services.AddScoped(typeof(IMediator), typeof(Mediator));

        services.AddEntryQueries();
        services.AddEntryCommands();
        services.AddQueries();
        services.AddCommands();
        services.AddAssemblers();
        services.AddCacheServices();

        return services;
    }

    private static IServiceCollection AddEntryQueries(this IServiceCollection services)
    {
        // Admins
        services.AddTransient<IRequestHandler<GetAdminByAddressQuery, AdminDto>, GetAdminByAddressQueryHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<GetBlocksWithFilterQuery, BlocksDto>, GetBlocksWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetBlockByHeightQuery, BlockDto>, GetBlockByHeightQueryHandler>();

        // Markets
        services.AddTransient<IRequestHandler<GetMarketsWithFilterQuery, MarketsDto>, GetMarketsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMarketByAddressQuery, MarketDto>, GetMarketByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<GetMarketSnapshotsWithFilterQuery, MarketSnapshotsDto>, GetMarketSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMarketPermissionsForAddressQuery, IEnumerable<MarketPermissionType>>, GetMarketPermissionsForAddressQueryHandler>();

        // Liquidity Pools
        services.AddTransient<IRequestHandler<GetLiquidityPoolsWithFilterQuery, LiquidityPoolsDto>, GetLiquidityPoolsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetLiquidityPoolSwapQuoteQuery, FixedDecimal>, GetLiquidityPoolSwapQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<GetSwapAmountInQuery, FixedDecimal>, GetSwapAmountInQueryHandler>();
        services.AddTransient<IRequestHandler<GetSwapAmountOutQuery, FixedDecimal>, GetSwapAmountOutQueryHandler>();
        services.AddTransient<IRequestHandler<GetLiquidityAmountInQuoteQuery, FixedDecimal>, GetLiquidityAmountInQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<GetLiquidityPoolSnapshotsWithFilterQuery, LiquidityPoolSnapshotsDto>, GetLiquidityPoolSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetLiquidityPoolByAddressQuery, LiquidityPoolDto>, GetLiquidityPoolByAddressQueryHandler>();

        // Mining Pools
        services.AddTransient<IRequestHandler<GetMiningPoolsWithFilterQuery, MiningPoolsDto>, GetMiningPoolsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMiningPoolByAddressQuery, MiningPoolDto>, GetMiningPoolByAddressQueryHandler>();

        // Vault
        services.AddTransient<IRequestHandler<GetVaultByAddressQuery, VaultDto>, GetVaultByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultsWithFilterQuery, VaultsDto>, GetVaultsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultProposalByVaultAddressAndPublicIdQuery, VaultProposalDto>, GetVaultProposalByVaultAddressAndPublicIdQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery, VaultProposalPledgeDto>, GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery, VaultProposalVoteDto>, GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultProposalsWithFilterQuery, VaultProposalsDto>, GetVaultProposalsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultProposalPledgesWithFilterQuery, VaultProposalPledgesDto>, GetVaultProposalPledgesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultProposalVotesWithFilterQuery, VaultProposalVotesDto>, GetVaultProposalVotesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetVaultCertificatesWithFilterQuery, VaultCertificatesDto>, GetVaultCertificatesWithFilterQueryHandler>();

        // Mining Governances
        services.AddTransient<IRequestHandler<GetMiningGovernancesWithFilterQuery, MiningGovernancesDto>, GetMiningGovernancesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMiningGovernanceByAddressQuery, MiningGovernanceDto>, GetMiningGovernanceByAddressQueryHandler>();

        // Tokens
        services.AddTransient<IRequestHandler<GetTokensWithFilterQuery, TokensDto>, GetTokensWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMarketTokensWithFilterQuery, MarketTokensDto>, GetMarketTokensWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetTokenSnapshotsWithFilterQuery, TokenSnapshotsDto>, GetTokenSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMarketTokenSnapshotsWithFilterQuery, MarketTokenSnapshotsDto>, GetMarketTokenSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetTokenByAddressQuery, TokenDto>, GetTokenByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<GetMarketTokenByMarketAndTokenAddressQuery, MarketTokenDto>, GetMarketTokenByMarketAndTokenAddressQueryHandler>();

        // Address Balances
        services.AddTransient<IRequestHandler<GetAddressBalanceByTokenQuery, AddressBalanceDto>, GetAddressBalanceByTokenQueryHandler>();
        services.AddTransient<IRequestHandler<GetStakingPositionByPoolQuery, StakingPositionDto>, GetStakingPositionByPoolQueryHandler>();
        services.AddTransient<IRequestHandler<GetMiningPositionByPoolQuery, MiningPositionDto>, GetMiningPositionByPoolQueryHandler>();
        services.AddTransient<IRequestHandler<GetAddressAllowanceQuery, AddressAllowanceDto>, GetAddressAllowanceQueryHandler>();
        services.AddTransient<IRequestHandler<GetAddressBalancesWithFilterQuery, AddressBalancesDto>, GetAddressBalancesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetMiningPositionsWithFilterQuery, MiningPositionsDto>, GetMiningPositionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetStakingPositionsWithFilterQuery, StakingPositionsDto>, GetStakingPositionsWithFilterQueryHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<GetBestBlockReceiptQuery, BlockReceipt>, GetBestBlockReceiptQueryHandler>();

        // Indexer
        services.AddTransient<IRequestHandler<GetIndexerStatusQuery, IndexerStatusDto>, GetIndexerStatusQueryHandler>();
        services.AddTransient<IRequestHandler<GetBlockReceiptAtChainSplitCommand, BlockReceipt>, GetBlockReceiptAtChainSplitCommandHandler>();

        // Transactions
        services.AddTransient<IRequestHandler<GetTransactionsWithFilterQuery, TransactionsDto>, GetTransactionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<GetTransactionByHashQuery, TransactionDto>, GetTransactionByHashQueryHandler>();

        return services;
    }

    private static IServiceCollection AddEntryCommands(this IServiceCollection services)
    {
        // Address
        services.AddTransient<IRequestHandler<CreateRefreshAddressBalanceCommand, AddressBalanceDto>, CreateRefreshAddressBalanceCommandHandler>();

        // Indexer
        services.AddTransient<IRequestHandler<ProcessDailySnapshotRefreshCommand, Unit>, ProcessDailySnapshotRefreshCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindSnapshotsCommand, bool>, CreateRewindSnapshotsCommandHandler>();

        // Deployments
        services.AddTransient<IRequestHandler<ProcessCoreDeploymentTransactionCommand, Unit>, ProcessCoreDeploymentTransactionCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessGovernanceDeploymentTransactionCommand, Unit>, ProcessGovernanceDeploymentTransactionCommandHandler>();

        // Deployers
        services.AddTransient<IRequestHandler<CreateDeployerCommand, ulong>, CreateDeployerCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindDeployersCommand, bool>, CreateRewindDeployersCommandHandler>();

        // Transactions
        services.AddTransient<IRequestHandler<CreateTransactionCommand, bool>, CreateTransactionCommandHandler>();
        services.AddTransient<IRequestHandler<CreateTransactionBroadcastCommand, Sha256>, CreateTransactionBroadcastCommandHandler>();
        services.AddTransient<IRequestHandler<CreateTransactionQuoteCommand, TransactionQuoteDto>, CreateTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateNotifyUserOfTransactionBroadcastCommand, bool>, CreateNotifyUserOfTransactionBroadcastCommandHandler>();

        // Markets
        services.AddTransient<IRequestHandler<ProcessMarketSnapshotsCommand, Unit>, ProcessMarketSnapshotsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateClaimStandardMarketOwnershipTransactionQuoteCommand, TransactionQuoteDto>, CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCollectStandardMarketFeesTransactionQuoteCommand, TransactionQuoteDto>, CreateCollectStandardMarketFeesTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCreateStakingMarketTransactionQuoteCommand, TransactionQuoteDto>, CreateCreateStakingMarketTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCreateStandardMarketTransactionQuoteCommand, TransactionQuoteDto>, CreateCreateStandardMarketTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateSetStandardMarketOwnershipTransactionQuoteCommand, TransactionQuoteDto>, CreateSetStandardMarketOwnershipTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateSetStandardMarketPermissionsTransactionQuoteCommand, TransactionQuoteDto>, CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindMarketPermissionsCommand, bool>, CreateRewindMarketPermissionsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindMarketsCommand, bool>, CreateRewindMarketsCommandHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<CreateBlockCommand, bool>, CreateBlockCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindToBlockCommand, bool>, CreateRewindToBlockCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessLatestBlocksCommand, Unit>, ProcessLatestBlocksCommandHandler>();

        // Liquidity Pools
        services.AddTransient<IRequestHandler<ProcessLiquidityPoolSnapshotsByTransactionCommand, Unit>, ProcessLiquidityPoolSnapshotsByTransactionCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessLiquidityPoolSnapshotRefreshCommand, Unit>, ProcessLiquidityPoolSnapshotRefreshCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessStaleLiquidityPoolSnapshotsCommand, Unit>, ProcessStaleLiquidityPoolSnapshotsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCreateLiquidityPoolTransactionQuoteCommand, TransactionQuoteDto>, CreateCreateLiquidityPoolTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateAddLiquidityTransactionQuoteCommand, TransactionQuoteDto>, CreateAddLiquidityTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRemoveLiquidityTransactionQuoteCommand, TransactionQuoteDto>, CreateRemoveLiquidityTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateSkimTransactionQuoteCommand, TransactionQuoteDto>, CreateSkimTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateSyncTransactionQuoteCommand, TransactionQuoteDto>, CreateSyncTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateStartStakingTransactionQuoteCommand, TransactionQuoteDto>, CreateStartStakingTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateStopStakingTransactionQuoteCommand, TransactionQuoteDto>, CreateStopStakingTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCollectStakingRewardsTransactionQuoteCommand, TransactionQuoteDto>, CreateCollectStakingRewardsTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindLiquidityPoolDailySnapshotCommand, bool>, CreateRewindLiquidityPoolDailySnapshotCommandHandler>();

        // Mining Pools
        services.AddTransient<IRequestHandler<CreateStartMiningTransactionQuoteCommand, TransactionQuoteDto>, CreateStartMiningTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateStopMiningTransactionQuoteCommand, TransactionQuoteDto>, CreateStopMiningTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCollectMiningRewardsTransactionQuoteCommand, TransactionQuoteDto>, CreateCollectMiningRewardsTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindMiningPoolsCommand, bool>, CreateRewindMiningPoolsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateMiningPoolCommand, ulong>, CreateMiningPoolCommandHandler>();

        // Mining Governances
        services.AddTransient<IRequestHandler<CreateRewardMiningPoolsTransactionQuoteCommand, TransactionQuoteDto>, CreateRewardMiningPoolsTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateMiningGovernanceCommand, ulong>, CreateMiningGovernanceCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindMiningGovernancesAndNominationsCommand, bool>, CreateRewindMiningGovernancesAndNominationsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateMiningGovernanceNominationsCommand, bool>, CreateMiningGovernanceNominationsCommandHandler>();

        // Vault
        services.AddTransient<IRequestHandler<CreateVaultCommand, ulong>, CreateVaultCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindVaultsCommand, bool>, CreateRewindVaultsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindVaultProposalsCommand, bool>, CreateRewindVaultProposalsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindVaultProposalVotesCommand, bool>, CreateRewindVaultProposalVotesCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindVaultProposalPledgesCommand, bool>, CreateRewindVaultProposalPledgesCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindVaultCertificatesCommand, bool>, CreateRewindVaultCertificatesCommandHandler>();
        services.AddTransient<IRequestHandler<CreateVaultProposalCreateCertificateQuoteCommand, TransactionQuoteDto>, CreateVaultProposalCreateCertificateQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateVaultProposalRevokeCertificateQuoteCommand, TransactionQuoteDto>, CreateVaultProposalRevokeCertificateQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateVaultProposalMinimumPledgeQuoteCommand, TransactionQuoteDto>, CreateVaultProposalMinimumPledgeQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateVaultProposalMinimumVoteQuoteCommand, TransactionQuoteDto>, CreateVaultProposalMinimumVoteQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateVaultProposalPledgeQuoteCommand, TransactionQuoteDto>, CreateVaultProposalPledgeQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateVaultProposalVoteQuoteCommand, TransactionQuoteDto>, CreateVaultProposalVoteQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateWithdrawVaultProposalPledgeQuoteCommand, TransactionQuoteDto>, CreateWithdrawVaultProposalPledgeQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateWithdrawVaultProposalVoteQuoteCommand, TransactionQuoteDto>, CreateWithdrawVaultProposalVoteQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateCompleteVaultProposalQuoteCommand, TransactionQuoteDto>, CreateCompleteVaultProposalQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRedeemVaultCertificateQuoteCommand, TransactionQuoteDto>, CreateRedeemVaultCertificateQuoteCommandHandler>();

        // Tokens
        services.AddTransient<IRequestHandler<CreateCrsTokenSnapshotsCommand, bool>, CreateCrsTokenSnapshotsCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessSrcTokenSnapshotCommand, decimal>, ProcessSrcTokenSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessLpTokenSnapshotCommand, decimal>, ProcessLpTokenSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<CreateApproveAllowanceTransactionQuoteCommand, TransactionQuoteDto>, CreateApproveAllowanceTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateDistributeTokensTransactionQuoteCommand, TransactionQuoteDto>, CreateDistributeTokensTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindAddressBalancesCommand, bool>, CreateRewindAddressBalancesCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindMiningPositionsCommand, bool>, CreateRewindMiningPositionsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindStakingPositionsCommand, bool>, CreateRewindStakingPositionsCommandHandler>();
        services.AddTransient<IRequestHandler<CreateAddressBalanceCommand, ulong>, CreateAddressBalanceCommandHandler>();
        services.AddTransient<IRequestHandler<CreateTokenCommand, ulong>, CreateTokenCommandHandler>();
        services.AddTransient<IRequestHandler<CreateAddTokenCommand, TokenDto>, CreateAddTokenCommandHandler>();
        services.AddTransient<IRequestHandler<CreateRewindTokenDailySnapshotCommand, bool>, CreateRewindTokenDailySnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<CreateSwapTransactionQuoteCommand, TransactionQuoteDto>, CreateSwapTransactionQuoteCommandHandler>();

        // Transaction Log Processors
        services.AddTransient<IRequestHandler<ProcessCreateLiquidityPoolLogCommand, bool>, ProcessCreateLiquidityPoolLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessApprovalLogCommand, bool>, ProcessApprovalLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessTransferLogCommand, bool>, ProcessTransferLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessOwnershipTransferredLogCommand, bool>, ProcessOwnershipTransferredLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessSupplyChangeLogCommand, bool>, ProcessSupplyChangeLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessCreateMarketLogCommand, bool>, ProcessCreateMarketLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessDistributionLogCommand, bool>, ProcessDistributionLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessClaimPendingMarketOwnershipLogCommand, bool>, ProcessClaimPendingMarketOwnershipLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessSetPendingMarketOwnershipLogCommand, bool>, ProcessSetPendingMarketOwnershipLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessMineLogCommand, bool>, ProcessMineLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessNominationLogCommand, bool>, ProcessNominationLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessChangeMarketPermissionLogCommand, bool>, ProcessChangeMarketPermissionLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessMintLogCommand, bool>, ProcessMintLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessBurnLogCommand, bool>, ProcessBurnLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessSwapLogCommand, bool>, ProcessSwapLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessReservesLogCommand, bool>, ProcessReservesLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessCollectMiningRewardsLogCommand, bool>, ProcessCollectMiningRewardsLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessEnableMiningLogCommand, bool>, ProcessEnableMiningLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessStakeLogCommand, bool>, ProcessStakeLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessCollectStakingRewardsLogCommand, bool>, ProcessCollectStakingRewardsLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessRewardMiningPoolLogCommand, bool>, ProcessRewardMiningPoolLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessCreateVaultCertificateLogCommand, bool>, ProcessCreateVaultCertificateLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessRevokeVaultCertificateLogCommand, bool>, ProcessRevokeVaultCertificateLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessRedeemVaultCertificateLogCommand, bool>, ProcessRedeemVaultCertificateLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessClaimPendingDeployerOwnershipLogCommand, bool>, ProcessClaimPendingDeployerOwnershipLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessSetPendingDeployerOwnershipLogCommand, bool>, ProcessSetPendingDeployerOwnershipLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessCompleteVaultProposalLogCommand, bool>, ProcessCompleteVaultProposalLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessCreateVaultProposalLogCommand, bool>, ProcessCreateVaultProposalLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessVaultProposalPledgeLogCommand, bool>, ProcessVaultProposalPledgeLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessVaultProposalVoteLogCommand, bool>, ProcessVaultProposalVoteLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessVaultProposalWithdrawPledgeLogCommand, bool>, ProcessVaultProposalWithdrawPledgeLogCommandHandler>();
        services.AddTransient<IRequestHandler<ProcessVaultProposalWithdrawVoteLogCommand, bool>, ProcessVaultProposalWithdrawVoteLogCommandHandler>();

        return services;
    }

    private static IServiceCollection AddQueries(this IServiceCollection services)
    {
        // Admins
        services.AddTransient<IRequestHandler<RetrieveAdminByAddressQuery, Admin>, RetrieveAdminByAddressQueryHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, Block>, RetrieveLatestBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveCirrusBestBlockReceiptQuery, BlockReceipt>, RetrieveCirrusBestBlockReceiptQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveCirrusBlockReceiptByHashQuery, BlockReceipt>, RetrieveCirrusBlockReceiptByHashQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveBlocksWithFilterQuery, IEnumerable<Block>>, RetrieveBlocksWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveBlockByHeightQuery, Block>, RetrieveBlockByHeightQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveCirrusBlockHashByHeightQuery, Sha256>, RetrieveCirrusBlockHashByHeightQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveBlockByMedianTimeQuery, Block>, RetrieveBlockByMedianTimeQueryHandler>();

        // Deployers
        services.AddTransient<IRequestHandler<RetrieveActiveDeployerQuery, Deployer>, RetrieveActiveDeployerQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveDeployerByAddressQuery, Deployer>, RetrieveDeployerByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveDeployersByModifiedBlockQuery, IEnumerable<Deployer>>, RetrieveDeployersByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveDeployerContractSummaryQuery, DeployerContractSummary>, RetrieveDeployerContractSummaryQueryHandler>();

        // Markets
        services.AddTransient<IRequestHandler<RetrieveMarketsWithFilterQuery, IEnumerable<Market>>, RetrieveMarketsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketSnapshotWithFilterQuery, MarketSnapshot>, RetrieveMarketSnapshotWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshot>>, RetrieveMarketSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketByAddressQuery, Market>, RetrieveMarketByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketPermissionQuery, MarketPermission>, RetrieveMarketPermissionQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketPermissionsByUserQuery, IEnumerable<MarketPermissionType>>, RetrieveMarketPermissionsByUserQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketByIdQuery, Market>, RetrieveMarketByIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveActiveMarketRouterByMarketIdQuery, MarketRouter>, RetrieveActiveMarketRouterByMarketIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketRouterByAddressQuery, MarketRouter>, RetrieveMarketRouterByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveAllMarketsQuery, IEnumerable<Market>>, RetrieveAllMarketsQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketsByModifiedBlockQuery, IEnumerable<Market>>, RetrieveMarketsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketPermissionsByModifiedBlockQuery, IEnumerable<MarketPermission>>, RetrieveMarketPermissionsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketContractSummaryQuery, MarketContractSummary>, RetrieveMarketContractSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketContractPermissionSummaryQuery, MarketContractPermissionSummary>, RetrieveMarketContractPermissionSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMarketSummaryByMarketIdQuery, MarketSummary>, RetrieveMarketSummaryByMarketIdQueryHandler>();

        // Liquidity Pools
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>, RetrieveLiquidityPoolsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshot>>, RetrieveLiquidityPoolSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSnapshotWithFilterQuery, LiquidityPoolSnapshot>, RetrieveLiquidityPoolSnapshotWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, UInt256>, RetrieveLiquidityPoolSwapQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityAmountInQuoteQuery, UInt256>, RetrieveLiquidityAmountInQuoteQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByIdQuery, LiquidityPool>, RetrieveLiquidityPoolByIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery, LiquidityPool>, RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByAddressQuery, LiquidityPool>, RetrieveLiquidityPoolByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery, LiquidityPoolSummary>, RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery, IEnumerable<LiquidityPool>>, RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler>();

        // Routers
        services.AddTransient<IRequestHandler<RetrieveSwapAmountInQuery, UInt256>, RetrieveSwapAmountInQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveSwapAmountOutQuery, UInt256>, RetrieveSwapAmountOutQueryHandler>();

        // Mining Pools
        services.AddTransient<IRequestHandler<RetrieveMiningPoolsWithFilterQuery, IEnumerable<MiningPool>>, RetrieveMiningPoolsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPoolByIdQuery, MiningPool>, RetrieveMiningPoolByIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPoolByLiquidityPoolIdQuery, MiningPool>, RetrieveMiningPoolByLiquidityPoolIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPoolByAddressQuery, MiningPool>, RetrieveMiningPoolByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPoolsByModifiedBlockQuery, IEnumerable<MiningPool>>, RetrieveMiningPoolsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPoolContractSummaryQuery, MiningPoolContractSummary>, RetrieveMiningPoolContractSummaryQueryHandler>();

        // Tokens
        services.AddTransient<IRequestHandler<RetrieveTokensWithFilterQuery, IEnumerable<Token>>, RetrieveTokensWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshot>>, RetrieveTokenSnapshotsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenSnapshotWithFilterQuery, TokenSnapshot>, RetrieveTokenSnapshotWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveLatestTokenDistributionByTokenIdQuery, TokenDistribution>, RetrieveLatestTokenDistributionByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveDistributionsByTokenIdQuery, IEnumerable<TokenDistribution>>, RetrieveDistributionsByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveStakingTokenContractSummaryQuery, StakingTokenContractSummary>, RetrieveStakingTokenContractSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenByIdQuery, Token>, RetrieveTokenByIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenSummaryByTokenIdQuery, TokenSummary>, RetrieveTokenSummaryByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenSummaryByMarketAndTokenIdQuery, TokenSummary>, RetrieveTokenSummaryByMarketAndTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenAttributesByTokenIdQuery, IEnumerable<TokenAttribute>>, RetrieveTokenAttributesByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTokenWrappedByTokenIdQuery, TokenWrapped>, RetrieveTokenWrappedByTokenIdQueryHandler>();

        // Mining Governances
        services.AddTransient<IRequestHandler<RetrieveMiningGovernanceContractSummaryByAddressQuery, MiningGovernanceContractSummary>, RetrieveMiningGovernanceContractSummaryByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery, IEnumerable<MiningGovernanceNomination>>, RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveCirrusMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceContractNominationSummary>>, RetrieveCirrusMiningGovernanceNominationsQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningGovernancesWithFilterQuery, IEnumerable<MiningGovernance>>, RetrieveMiningGovernancesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningGovernanceByAddressQuery, MiningGovernance>, RetrieveMiningGovernanceByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningGovernanceByTokenIdQuery, MiningGovernance>, RetrieveMiningGovernanceByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>, RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningGovernancesByModifiedBlockQuery, IEnumerable<MiningGovernance>>, RetrieveMiningGovernancesByModifiedBlockQueryHandler>();

        // Vault
        services.AddTransient<IRequestHandler<RetrieveVaultCertificatesByModifiedBlockQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultContractSummaryQuery, VaultContractSummary>, RetrieveVaultContractSummaryQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultByIdQuery, Vault>, RetrieveVaultByIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultByAddressQuery, Vault>, RetrieveVaultByAddressQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultsWithFilterQuery, IEnumerable<Vault>>, RetrieveVaultsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery, VaultProposalVote>, RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery, VaultProposalPledge>, RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalByVaultIdAndPublicIdQuery, VaultProposal>, RetrieveVaultProposalByVaultIdAndPublicIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultCertificatesByVaultIdAndOwnerQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesByVaultIdAndOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalByIdQuery, VaultProposal>, RetrieveVaultProposalByIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultByTokenIdQuery, Vault>, RetrieveVaultByTokenIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultsByModifiedBlockQuery, IEnumerable<Vault>>, RetrieveVaultsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalsByModifiedBlockQuery, IEnumerable<VaultProposal>>, RetrieveVaultProposalsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalVotesByModifiedBlockQuery, IEnumerable<VaultProposalVote>>, RetrieveVaultProposalVotesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalPledgesByModifiedBlockQuery, IEnumerable<VaultProposalPledge>>, RetrieveVaultProposalPledgesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultContractCertificateSummaryByOwnerQuery, VaultContractCertificateSummary>, RetrieveVaultContractCertificateSummaryByOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalsWithFilterQuery, IEnumerable<VaultProposal>>, RetrieveVaultProposalsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalPledgesWithFilterQuery, IEnumerable<VaultProposalPledge>>, RetrieveVaultProposalPledgesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalVotesWithFilterQuery, IEnumerable<VaultProposalVote>>, RetrieveVaultProposalVotesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalCertificatesByCertificateIdQuery, IEnumerable<VaultProposalCertificate>>, RetrieveVaultProposalCertificatesByCertificateIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultProposalCertificateByProposalIdQuery, VaultProposalCertificate>, RetrieveVaultProposalCertificateByProposalIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveVaultCertificateByIdQuery, VaultCertificate>, RetrieveVaultCertificateByIdQueryHandler>();

        // Transactions
        services.AddTransient<IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>, RetrieveCirrusTransactionByHashQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTransactionsWithFilterQuery, IEnumerable<Transaction>>, RetrieveTransactionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, RetrieveTransactionLogsByTransactionIdQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveTransactionByHashQuery, Transaction>, RetrieveTransactionByHashQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveCirrusUnverifiedTransactionSenderByHashQuery, Address>, RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandler>();

        // Address Balances
        services.AddTransient<IRequestHandler<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>, RetrieveAddressStakingByLiquidityPoolIdAndOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>, RetrieveAddressMiningByMiningPoolIdAndOwnerQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveAddressBalanceByOwnerAndTokenQuery, AddressBalance>, RetrieveAddressBalanceByOwnerAndTokenQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveAddressAllowanceQuery, AddressAllowance>, RetrieveAddressAllowanceQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveAddressBalancesWithFilterQuery, IEnumerable<AddressBalance>>, RetrieveAddressBalancesWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPositionsWithFilterQuery, IEnumerable<AddressMining>>, RetrieveMiningPositionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveStakingPositionsWithFilterQuery, IEnumerable<AddressStaking>>, RetrieveStakingPositionsWithFilterQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveAddressBalancesByModifiedBlockQuery, IEnumerable<AddressBalance>>, RetrieveAddressBalancesByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveMiningPositionsByModifiedBlockQuery, IEnumerable<AddressMining>>, RetrieveMiningPositionsByModifiedBlockQueryHandler>();
        services.AddTransient<IRequestHandler<RetrieveStakingPositionsByModifiedBlockQuery, IEnumerable<AddressStaking>>, RetrieveStakingPositionsByModifiedBlockQueryHandler>();

        // Indexer
        services.AddTransient<IRequestHandler<RetrieveIndexerLockQuery, IndexLock>, RetrieveIndexerLockQueryHandler>();

        return services;
    }

    private static IServiceCollection AddCommands(this IServiceCollection services)
    {
        // Auth
        services.AddTransient<IRequestHandler<MakeAuthSuccessCommand, bool>, MakeAuthSuccessCommandHandler>();

        // Blocks
        services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
        services.AddTransient<IRequestHandler<MakeRewindToBlockCommand, bool>, MakeRewindToBlockCommandHandler>();

        // Indexer
        services.AddTransient<IRequestHandler<MakeIndexerLockCommand, bool>, MakeIndexerLockCommandHandler>();
        services.AddTransient<IRequestHandler<MakeIndexerUnlockCommand, Unit>, MakeIndexerUnlockCommandHandler>();

        // Tokens
        services.AddTransient<IRequestHandler<MakeTokenCommand, ulong>, MakeTokenCommandHandler>();
        services.AddTransient<IRequestHandler<MakeTokenSnapshotCommand, bool>, MakeTokenSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<MakeTokenDistributionCommand, bool>, MakeTokenDistributionCommandHandler>();
        services.AddTransient<IRequestHandler<MakeTokenAttributeCommand, bool>, MakeTokenAttributeCommandHandler>();
        services.AddTransient<IRequestHandler<MakeTokenWrappedCommand, ulong>, MakeTokenWrappedCommandHandler>();

        // Transactions
        services.AddTransient<IRequestHandler<MakeTransactionCommand, ulong>, MakeTransactionCommandHandler>();
        services.AddTransient<IRequestHandler<MakeTransactionQuoteCommand, TransactionQuote>, MakeTransactionQuoteCommandHandler>();
        services.AddTransient<IRequestHandler<MakeNotifyUserOfTransactionBroadcastCommand, Unit>, MakeNotifyUserOfTransactionBroadcastCommandHandler>();

        // Liquidity Pools
        services.AddTransient<IRequestHandler<MakeLiquidityPoolCommand, ulong>, MakeLiquidityPoolCommandHandler>();
        services.AddTransient<IRequestHandler<MakeLiquidityPoolSnapshotCommand, bool>, MakeLiquidityPoolSnapshotCommandHandler>();
        services.AddTransient<IRequestHandler<MakeLiquidityPoolSummaryCommand, ulong>, MakeLiquidityPoolSummaryCommandHandler>();

        // Mining Pools
        services.AddTransient<IRequestHandler<MakeMiningPoolCommand, ulong>, MakeMiningPoolCommandHandler>();

        // Markets
        services.AddTransient<IRequestHandler<MakeMarketCommand, ulong>, MakeMarketCommandHandler>();
        services.AddTransient<IRequestHandler<MakeMarketPermissionCommand, ulong>, MakeMarketPermissionCommandHandler>();
        services.AddTransient<IRequestHandler<MakeMarketSummaryCommand, ulong>, MakeMarketSummaryCommandHandler>();

        services.AddTransient<IRequestHandler<MakeMarketRouterCommand, bool>, MakeMarketRouterCommandHandler>();
        services.AddTransient<IRequestHandler<MakeMarketSnapshotCommand, bool>, MakeMarketSnapshotCommandHandler>();

        // Deployers
        services.AddTransient<IRequestHandler<MakeDeployerCommand, ulong>, MakeDeployerCommandHandler>();

        // Mining Governances
        services.AddTransient<IRequestHandler<MakeMiningGovernanceCommand, ulong>, MakeMiningGovernanceCommandHandler>();
        services.AddTransient<IRequestHandler<MakeMiningGovernanceNominationCommand, ulong>, MakeMiningGovernanceNominationCommandHandler>();
        services.AddTransient<IRequestHandler<MakeMiningGovernanceNominationsCommand, bool>, MakeMiningGovernanceNominationsCommandHandler>();

        // Vault
        services.AddTransient<IRequestHandler<MakeVaultCommand, ulong>, MakeVaultCommandHandler>();
        services.AddTransient<IRequestHandler<MakeVaultCertificateCommand, ulong>, MakeVaultCertificateCommandHandler>();
        services.AddTransient<IRequestHandler<MakeVaultProposalCommand, ulong>, MakeVaultProposalCommandHandler>();
        services.AddTransient<IRequestHandler<MakeVaultProposalPledgeCommand, ulong>, MakeVaultProposalPledgeCommandHandler>();
        services.AddTransient<IRequestHandler<MakeVaultProposalVoteCommand, ulong>, MakeVaultProposalVoteCommandHandler>();
        services.AddTransient<IRequestHandler<MakeVaultProposalCertificateCommand, ulong>, MakeVaultProposalCertificateCommandHandler>();

        // Wallet Address
        services.AddTransient<IRequestHandler<MakeAddressBalanceCommand, ulong>, MakeAddressBalanceCommandHandler>();
        services.AddTransient<IRequestHandler<MakeAddressStakingCommand, ulong>, MakeAddressStakingCommandHandler>();
        services.AddTransient<IRequestHandler<MakeAddressMiningCommand, ulong>, MakeAddressMiningCommandHandler>();
        services.AddTransient<IRequestHandler<MakeTransactionBroadcastCommand, Sha256>, MakeTransactionBroadcastCommandHandler>(); // Keep this one around

        return services;
    }

    private static IServiceCollection AddAssemblers(this IServiceCollection services)
    {
        // Wallet Addresses
        services.AddTransient<IModelAssembler<AddressAllowance, AddressAllowanceDto>, AddressAllowanceDtoAssembler>();
        services.AddTransient<IModelAssembler<AddressBalance, AddressBalanceDto>, AddressBalanceDtoAssembler>();
        services.AddTransient<IModelAssembler<AddressMining, MiningPositionDto>, MiningPositionDtoAssembler>();
        services.AddTransient<IModelAssembler<AddressStaking, StakingPositionDto>, StakingPositionDtoAssembler>();

        // Liquidity Pools
        services.AddTransient<IModelAssembler<LiquidityPool, LiquidityPoolDto>, LiquidityPoolDtoAssembler>();
        services.AddTransient<IModelAssembler<IList<LiquidityPoolSnapshot>, IEnumerable<LiquidityPoolSnapshotDto>>, LiquidityPoolSnapshotsDtosAssembler>();

        // Markets
        services.AddTransient<IModelAssembler<Market, MarketDto>, MarketDtoAssembler>();

        // Mining Pools
        services.AddTransient<IModelAssembler<MiningPool, MiningPoolDto>, MiningPoolDtoAssembler>();

        // Tokens
        services.AddTransient<IModelAssembler<Token, TokenDto>, TokenDtoAssembler>();
        services.AddTransient<IModelAssembler<MarketToken, MarketTokenDto>, MarketTokenDtoAssembler>();

        // Mining Governances
        services.AddTransient<IModelAssembler<MiningGovernance, MiningGovernanceDto>, MiningGovernanceDtoAssembler>();

        // Vaults
        services.AddTransient<IModelAssembler<VaultCertificate, VaultCertificateDto>, VaultCertificateDtoAssembler>();
        services.AddTransient<IModelAssembler<Vault, VaultDto>, VaultDtoAssembler>();
        services.AddTransient<IModelAssembler<VaultProposal, VaultProposalDto>, VaultProposalDtoAssembler>();
        services.AddTransient<IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto>, VaultProposalPledgeDtoAssembler>();
        services.AddTransient<IModelAssembler<VaultProposalVote, VaultProposalVoteDto>, VaultProposalVoteDtoAssembler>();

        // Transactions
        services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
        services.AddTransient<IModelAssembler<TransactionQuote, TransactionQuoteDto>, TransactionQuoteDtoAssembler>();
        services.AddTransient<IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>>, TransactionEventsDtoAssembler>();

        // Liquidity Pool Logs
        services.AddTransient<IModelAssembler<ReservesLog, ReservesChangeEventDto>, ReservesChangeEventDtoAssembler>();
        services.AddTransient<IModelAssembler<SwapLog, SwapEventDto>, SwapEventDtoAssembler>();
        services.AddTransient<IModelAssembler<BurnLog, RemoveLiquidityEventDto>, RemoveLiquidityEventDtoAssembler>();
        services.AddTransient<IModelAssembler<MintLog, AddLiquidityEventDto>, AddLiquidityEventDtoAssembler>();

        // Token Logs
        services.AddTransient<IModelAssembler<TransferLog, TransferEventDto>, TransferEventDtoAssembler>();
        services.AddTransient<IModelAssembler<ApprovalLog, ApprovalEventDto>, ApprovalEventDtoAssembler>();

        // Interflux Token Logs
        services.AddTransient<IModelAssembler<SupplyChangeLog, SupplyChangeEventDto>, SupplyChangeEventDtoAssembler>();

        return services;
    }

    private static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddSingleton<IWrappedTokenTrustValidator, WrappedTokenTrustValidator>();
        return services;
    }
}
