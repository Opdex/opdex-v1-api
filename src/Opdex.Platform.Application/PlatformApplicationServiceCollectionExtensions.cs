

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
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
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Admins;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.Admins;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.Assemblers.TransactionEvents;
using Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens;
using Opdex.Platform.Application.EntryHandlers;
using Opdex.Platform.Application.EntryHandlers.Addresses.Allowances;
using Opdex.Platform.Application.EntryHandlers.Addresses.Balances;
using Opdex.Platform.Application.EntryHandlers.Addresses.Mining;
using Opdex.Platform.Application.EntryHandlers.Addresses.Staking;
using Opdex.Platform.Application.EntryHandlers.Admins;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Application.EntryHandlers.Deployers;
using Opdex.Platform.Application.EntryHandlers.Governances;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Application.EntryHandlers.Markets.Permissions;
using Opdex.Platform.Application.EntryHandlers.Markets.Quotes;
using Opdex.Platform.Application.EntryHandlers.Markets.Snapshots;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;
using Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults.Certificates;
using Opdex.Platform.Application.EntryHandlers.Vaults.Quotes;
using Opdex.Platform.Application.Handlers;
using Opdex.Platform.Application.Handlers.Addresses.Allowances;
using Opdex.Platform.Application.Handlers.Addresses.Balances;
using Opdex.Platform.Application.Handlers.Addresses.Mining;
using Opdex.Platform.Application.Handlers.Addresses.Staking;
using Opdex.Platform.Application.Handlers.Admins;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Application.Handlers.Deployers;
using Opdex.Platform.Application.Handlers.Governances;
using Opdex.Platform.Application.Handlers.Governances.Nominations;
using Opdex.Platform.Application.Handlers.Indexer;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Handlers.Markets;
using Opdex.Platform.Application.Handlers.Markets.Permissions;
using Opdex.Platform.Application.Handlers.Markets.Snapshots;
using Opdex.Platform.Application.Handlers.MiningPools;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Application.Handlers.Tokens.Distribution;
using Opdex.Platform.Application.Handlers.Tokens.Snapshots;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Application.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Application.Handlers.Vaults;
using Opdex.Platform.Application.Handlers.Vaults.Certificates;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Admins;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Collections.Generic;

namespace Opdex.Platform.Application
{
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

            return services;
        }

        private static IServiceCollection AddEntryQueries(this IServiceCollection services)
        {
            // Admins
            services.AddTransient<IRequestHandler<GetAdminByAddressQuery, AdminDto>, GetAdminByAddressQueryHandler>();

            // Markets
            services.AddTransient<IRequestHandler<GetMarketByAddressQuery, MarketDto>, GetMarketByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshotDto>>, GetMarketSnapshotsWithFilterQueryHandler>();

            // Liquidity Pools
            services.AddTransient<IRequestHandler<GetLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPoolDto>>, GetLiquidityPoolsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolSwapQuoteQuery, FixedDecimal>, GetLiquidityPoolSwapQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolAddLiquidityQuoteQuery, FixedDecimal>, GetLiquidityPoolAddLiquidityQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshotDto>>, GetLiquidityPoolSnapshotsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolByAddressQuery, LiquidityPoolDto>, GetLiquidityPoolByAddressQueryHandler>();

            // Mining Pools
            services.AddTransient<IRequestHandler<GetMiningPoolsWithFilterQuery, MiningPoolsDto>, GetMiningPoolsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetMiningPoolByAddressQuery, MiningPoolDto>, GetMiningPoolByAddressQueryHandler>();

            // Vaults
            services.AddTransient<IRequestHandler<GetVaultsWithFilterQuery, VaultsDto>, GetVaultsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetVaultByAddressQuery, VaultDto>, GetVaultByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetVaultCertificatesWithFilterQuery, VaultCertificatesDto>, GetVaultCertificatesWithFilterQueryHandler>();

            // Governances
            services.AddTransient<IRequestHandler<GetMiningGovernancesWithFilterQuery, MiningGovernancesDto>, GetMiningGovernancesWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetMiningGovernanceByAddressQuery, MiningGovernanceDto>, GetMiningGovernanceByAddressQueryHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<GetTokensWithFilterQuery, IEnumerable<TokenDto>>, GetTokensWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshotDto>>, GetTokenSnapshotsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenByAddressQuery, TokenDto>, GetTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenByAddressFromFullNodeQuery, TokenDto>, GetTokenByAddressFromFullNodeQueryHandler>();

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

            // Transactions
            services.AddTransient<IRequestHandler<GetTransactionsWithFilterQuery, TransactionsDto>, GetTransactionsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetTransactionByHashQuery, TransactionDto>, GetTransactionByHashQueryHandler>();

            return services;
        }

        private static IServiceCollection AddEntryCommands(this IServiceCollection services)
        {
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
            services.AddTransient<IRequestHandler<CreateTransactionBroadcastCommand, string>, CreateTransactionBroadcastCommandHandler>();
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
            services.AddTransient<IRequestHandler<ProcessDailyLiquidityPoolSnapshotRefreshCommand, Unit>, ProcessDailyLiquidityPoolSnapshotRefreshCommandHandler>();
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

            // Vaults
            services.AddTransient<IRequestHandler<CreateSetPendingVaultOwnershipTransactionQuoteCommand, TransactionQuoteDto>, CreateSetPendingVaultOwnershipTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateClaimPendingVaultOwnershipTransactionQuoteCommand, TransactionQuoteDto>, CreateClaimPendingVaultOwnershipTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateCreateVaultCertificateTransactionQuoteCommand, TransactionQuoteDto>, CreateCreateVaultCertificateTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRedeemVaultCertificatesTransactionQuoteCommand, TransactionQuoteDto>, CreateRedeemVaultCertificatesTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRevokeVaultCertificatesTransactionQuoteCommand, TransactionQuoteDto>, CreateRevokeVaultCertificatesTransactionQuoteCommandHandler>();

            // Governances
            services.AddTransient<IRequestHandler<CreateRewardMiningPoolsTransactionQuoteCommand, TransactionQuoteDto>, CreateRewardMiningPoolsTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateMiningGovernanceCommand, ulong>, CreateMiningGovernanceCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRewindMiningGovernancesAndNominationsCommand, bool>, CreateRewindMiningGovernancesAndNominationsCommandHandler>();
            services.AddTransient<IRequestHandler<CreateGovernanceNominationsCommand, bool>, CreateGovernanceNominationsCommandHandler>();

            // Vaults
            services.AddTransient<IRequestHandler<CreateRewindVaultsCommand, bool>, CreateRewindVaultsCommandHandler>();
            services.AddTransient<IRequestHandler<CreateVaultCommand, ulong>, CreateVaultCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRewindVaultCertificatesCommand, bool>, CreateRewindVaultCertificatesCommandHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<CreateCrsTokenSnapshotsCommand, Unit>, CreateCrsTokenSnapshotsCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessSrcTokenSnapshotCommand, decimal>, ProcessSrcTokenSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessLpTokenSnapshotCommand, decimal>, ProcessLpTokenSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<CreateApproveAllowanceTransactionQuoteCommand, TransactionQuoteDto>, CreateApproveAllowanceTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateDistributeTokensTransactionQuoteCommand, TransactionQuoteDto>, CreateDistributeTokensTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRewindAddressBalancesCommand, bool>, CreateRewindAddressBalancesCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRewindMiningPositionsCommand, bool>, CreateRewindMiningPositionsCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRewindStakingPositionsCommand, bool>, CreateRewindStakingPositionsCommandHandler>();
            services.AddTransient<IRequestHandler<CreateAddressBalanceCommand, ulong>, CreateAddressBalanceCommandHandler>();
            services.AddTransient<IRequestHandler<CreateTokenCommand, ulong>, CreateTokenCommandHandler>();
            services.AddTransient<IRequestHandler<CreateRewindTokenDailySnapshotCommand, bool>, CreateRewindTokenDailySnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<CreateSwapTransactionQuoteCommand, TransactionQuoteDto>, CreateSwapTransactionQuoteCommandHandler>();

            // Transaction Log Processors
            services.AddTransient<IRequestHandler<ProcessCreateLiquidityPoolLogCommand, bool>, ProcessCreateLiquidityPoolLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessApprovalLogCommand, bool>, ProcessApprovalLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessTransferLogCommand, bool>, ProcessTransferLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessCreateMarketLogCommand, bool>, ProcessCreateMarketLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessDistributionLogCommand, bool>, ProcessDistributionLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessClaimPendingVaultOwnershipLogCommand, bool>, ProcessClaimPendingVaultOwnershipLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessSetPendingVaultOwnershipLogCommand, bool>, ProcessSetPendingVaultOwnershipLogCommandHandler>();
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
            services.AddTransient<IRequestHandler<RetrieveBlockByHeightQuery, Block>, RetrieveBlockByHeightQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusBlockHashByHeightQuery, string>, RetrieveCirrusBlockHashByHeightQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveBlockByMedianTimeQuery, Block>, RetrieveBlockByMedianTimeQueryHandler>();

            // Deployers
            services.AddTransient<IRequestHandler<RetrieveActiveDeployerQuery, Deployer>, RetrieveActiveDeployerQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveDeployerByAddressQuery, Deployer>, RetrieveDeployerByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveDeployersByModifiedBlockQuery, IEnumerable<Deployer>>, RetrieveDeployersByModifiedBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveDeployerContractSummaryQuery, DeployerContractSummary>, RetrieveDeployerContractSummaryQueryHandler>();

            // Markets
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

            // Liquidity Pools
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>, RetrieveLiquidityPoolsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSnapshotsWithFilterQuery, IEnumerable<LiquidityPoolSnapshot>>, RetrieveLiquidityPoolSnapshotsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSnapshotWithFilterQuery, LiquidityPoolSnapshot>, RetrieveLiquidityPoolSnapshotWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, UInt256>, RetrieveLiquidityPoolSwapQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolAddLiquidityQuoteQuery, UInt256>, RetrieveLiquidityPoolAddLiquidityQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByLpTokenIdQuery, LiquidityPool>, RetrieveLiquidityPoolByLpTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByIdQuery, LiquidityPool>, RetrieveLiquidityPoolByIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery, LiquidityPool>, RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByAddressQuery, LiquidityPool>, RetrieveLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery, LiquidityPoolSummary>, RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandler>();

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
            services.AddTransient<IRequestHandler<RetrieveLatestTokenDistributionQuery, TokenDistribution>, RetrieveLatestTokenDistributionQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveStakingTokenContractSummaryQuery, StakingTokenContractSummary>, RetrieveStakingTokenContractSummaryQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTokenByIdQuery, Token>, RetrieveTokenByIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();

            // Governances
            services.AddTransient<IRequestHandler<RetrieveMiningGovernanceContractSummaryByAddressQuery, MiningGovernanceContractSummary>, RetrieveMiningGovernanceContractSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveActiveGovernanceNominationsByGovernanceIdQuery, IEnumerable<MiningGovernanceNomination>>, RetrieveActiveGovernanceNominationsByGovernanceIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusMiningGovernanceNominationsQuery, IEnumerable<GovernanceContractNominationSummary>>, RetrieveCirrusMiningGovernanceNominationsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernancesWithFilterQuery, IEnumerable<MiningGovernance>>, RetrieveMiningGovernancesWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernanceByAddressQuery, MiningGovernance>, RetrieveMiningGovernanceByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernanceByTokenIdQuery, MiningGovernance>, RetrieveMiningGovernanceByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>, RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernancesByModifiedBlockQuery, IEnumerable<MiningGovernance>>, RetrieveMiningGovernancesByModifiedBlockQueryHandler>();

            // Vaults
            services.AddTransient<IRequestHandler<RetrieveVaultsWithFilterQuery, IEnumerable<Vault>>, RetrieveVaultsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultByAddressQuery, Vault>, RetrieveVaultByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultByTokenIdQuery, Vault>, RetrieveVaultByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultCertificatesByOwnerAddressQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesByOwnerAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultContractSummaryQuery, VaultContractSummary>, RetrieveVaultContractSummaryQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultsByModifiedBlockQuery, IEnumerable<Vault>>, RetrieveVaultsByModifiedBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultCertificatesByModifiedBlockQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesByModifiedBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultByIdQuery, Vault>, RetrieveVaultByIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultContractCertificateSummariesByOwnerQuery, IEnumerable<VaultContractCertificateSummary>>, RetrieveVaultContractCertificateSummariesByOwnerQueryHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>, RetrieveCirrusTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionsWithFilterQuery, IEnumerable<Transaction>>, RetrieveTransactionsWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, RetrieveTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionByHashQuery, Transaction>, RetrieveTransactionByHashQueryHandler>();

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

            // CMC
            services.AddTransient<IRequestHandler<RetrieveCmcStraxPriceQuery, decimal>, RetrieveCmcStraxPriceQueryHandler>();

            // Indexer
            services.AddTransient<IRequestHandler<RetrieveIndexerLockQuery, IndexLock>, RetrieveIndexerLockQueryHandler>();

            return services;
        }

        private static IServiceCollection AddCommands(this IServiceCollection services)
        {
            // Blocks
            services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeRewindToBlockCommand, bool>, MakeRewindToBlockCommandHandler>();

            // Indexer
            services.AddTransient<IRequestHandler<MakeIndexerLockCommand, Unit>, MakeIndexerLockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeIndexerUnlockCommand, Unit>, MakeIndexerUnlockCommandHandler>();

            // Tokens
            services.AddTransient<IRequestHandler<MakeTokenCommand, ulong>, MakeTokenCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenSnapshotCommand, bool>, MakeTokenSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenDistributionCommand, bool>, MakeTokenDistributionCommandHandler>();

            // Transactions
            services.AddTransient<IRequestHandler<MakeTransactionCommand, ulong>, MakeTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionQuoteCommand, TransactionQuote>, MakeTransactionQuoteCommandHandler>();
            services.AddTransient<IRequestHandler<MakeNotifyUserOfTransactionBroadcastCommand, bool>, MakeNotifyUserOfTransactionBroadcastCommandHandler>();

            // Liquidity Pools
            services.AddTransient<IRequestHandler<MakeLiquidityPoolCommand, ulong>, MakeLiquidityPoolCommandHandler>();
            services.AddTransient<IRequestHandler<MakeLiquidityPoolSnapshotCommand, bool>, MakeLiquidityPoolSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<MakeLiquidityPoolSummaryCommand, ulong>, MakeLiquidityPoolSummaryCommandHandler>();

            // Mining Pools
            services.AddTransient<IRequestHandler<MakeMiningPoolCommand, ulong>, MakeMiningPoolCommandHandler>();

            // Markets
            services.AddTransient<IRequestHandler<MakeMarketCommand, ulong>, MakeMarketCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMarketPermissionCommand, ulong>, MakeMarketPermissionCommandHandler>();

            services.AddTransient<IRequestHandler<MakeMarketRouterCommand, bool>, MakeMarketRouterCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMarketSnapshotCommand, bool>, MakeMarketSnapshotCommandHandler>();

            // Deployers
            services.AddTransient<IRequestHandler<MakeDeployerCommand, ulong>, MakeDeployerCommandHandler>();

            // Governances
            services.AddTransient<IRequestHandler<MakeMiningGovernanceCommand, ulong>, MakeMiningGovernanceCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMiningGovernanceNominationCommand, ulong>, MakeMiningGovernanceNominationCommandHandler>();
            services.AddTransient<IRequestHandler<MakeGovernanceNominationsCommand, bool>, MakeGovernanceNominationsCommandHandler>();

            // Vaults
            services.AddTransient<IRequestHandler<MakeVaultCommand, ulong>, MakeVaultCommandHandler>();
            services.AddTransient<IRequestHandler<MakeVaultCertificateCommand, bool>, MakeVaultCertificateCommandHandler>();

            // Wallet Address
            services.AddTransient<IRequestHandler<MakeAddressBalanceCommand, ulong>, MakeAddressBalanceCommandHandler>();
            services.AddTransient<IRequestHandler<MakeAddressStakingCommand, ulong>, MakeAddressStakingCommandHandler>();
            services.AddTransient<IRequestHandler<MakeAddressMiningCommand, ulong>, MakeAddressMiningCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionBroadcastCommand, string>, MakeTransactionBroadcastCommandHandler>(); // Keep this one around

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

            // Markets
            services.AddTransient<IModelAssembler<Market, MarketDto>, MarketDtoAssembler>();

            // Mining Pools
            services.AddTransient<IModelAssembler<MiningPool, MiningPoolDto>, MiningPoolDtoAssembler>();

            // Tokens
            services.AddTransient<IModelAssembler<Token, TokenDto>, TokenDtoAssembler>();

            // Governances
            services.AddTransient<IModelAssembler<MiningGovernance, MiningGovernanceDto>, MiningGovernanceDtoAssembler>();

            // Vaults
            services.AddTransient<IModelAssembler<Vault, VaultDto>, VaultDtoAssembler>();

            // Transactions
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            services.AddTransient<IModelAssembler<TransactionQuote, TransactionQuoteDto>, TransactionQuoteDtoAssembler>();
            services.AddTransient<IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>>, TransactionEventsDtoAssembler>();

            // Liquidity Pool Logs
            services.AddTransient<IModelAssembler<SwapLog, SwapEventDto>, SwapEventDtoAssembler>();
            services.AddTransient<IModelAssembler<BurnLog, RemoveLiquidityEventDto>, RemoveLiquidityEventDtoAssembler>();
            services.AddTransient<IModelAssembler<MintLog, AddLiquidityEventDto>, AddLiquidityEventDtoAssembler>();

            // Token Logs
            services.AddTransient<IModelAssembler<TransferLog, TransferEventDto>, TransferEventDtoAssembler>();
            services.AddTransient<IModelAssembler<ApprovalLog, ApprovalEventDto>, ApprovalEventDtoAssembler>();

            return services;
        }
    }
}
