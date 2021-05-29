using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Application.EntryHandlers.Pools;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vault;
using Opdex.Platform.Application.EntryHandlers.Transactions.Wallet;
using Opdex.Platform.Application.Handlers;
using Opdex.Platform.Application.Handlers.Addresses;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Application.Handlers.Deployers;
using Opdex.Platform.Application.Handlers.Markets;
using Opdex.Platform.Application.Handlers.MiningGovernance;
using Opdex.Platform.Application.Handlers.Pools;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Application.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Application.Handlers.Transactions.Wallet;
using Opdex.Platform.Application.Handlers.vault;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using TokenDto = Opdex.Platform.Application.Abstractions.Models.TokenDto;

namespace Opdex.Platform.Application
{
    public static class PlatformApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            services.AddScoped(typeof(IMediator), typeof(Mediator));

            // Entry Queries
            services.AddTransient<IRequestHandler<GetAllPoolsQuery, IEnumerable<LiquidityPoolDto>>, GetAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllTokensQuery, IEnumerable<TokenDto>>, GetAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<GetLatestMarketSnapshotQuery, MarketSnapshotDto>, GetLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<GetTransactionsByPoolWithFilterQuery, IEnumerable<TransactionDto>>, GetTransactionsByPoolWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolSwapQuoteQuery, string>, GetLiquidityPoolSwapQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolAddLiquidityQuoteQuery, string>, GetLiquidityPoolAddLiquidityQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<GetBestBlockQuery, BlockReceiptDto>, GetBestBlockQueryHandler>();

            // Queries
            services.AddTransient<IRequestHandler<RetrieveAllPoolsQuery, IEnumerable<LiquidityPool>>, RetrieveAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAllTokensQuery, IEnumerable<Token>>, RetrieveAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLatestMarketSnapshotQuery, MarketSnapshot>, RetrieveLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>, RetrieveTransactionsByPoolWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockReceiptDto>, RetrieveCirrusCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusBlockByHashQuery, BlockReceiptDto>, RetrieveCirrusBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>, RetrieveCirrusTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCmcStraxPriceQuery, decimal>, RetrieveCmcStraxPriceQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery, List<TokenSnapshot>>, RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery, IEnumerable<LiquidityPoolSnapshot>>, RetrieveActiveLiquidityPoolSnapshotsByPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveActiveMarketSnapshotsByMarketIdQuery, IEnumerable<MarketSnapshot>>, RetrieveActiveMarketSnapshotsByMarketIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveDeployerByAddressQuery, Deployer>, RetrieveDeployerByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLatestTokenSnapshotByTokenIdQuery, TokenSnapshot>, RetrieveLatestTokenSnapshotByTokenIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveBlockByHeightQuery, Block>, RetrieveBlockByHeightQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMarketByAddressQuery, Market>, RetrieveMarketByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, string>, RetrieveLiquidityPoolSwapQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolAddLiquidityQuoteQuery, string>, RetrieveLiquidityPoolAddLiquidityQuoteQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningPoolByLiquidityPoolIdQuery, MiningPool>, RetrieveMiningPoolByLiquidityPoolIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernanceQuery, MiningGovernance>, RetrieveMiningGovernanceQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLatestTokenDistributionQuery, TokenDistribution>, RetrieveLatestTokenDistributionQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningGovernanceContractSummaryByAddressQuery, MiningGovernanceContractSummary>, RetrieveMiningGovernanceContractSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveStakingTokenContractSummaryByAddressQuery, StakingTokenContractSummary>, RetrieveStakingTokenContractSummaryByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultQuery, Vault>, RetrieveVaultQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveVaultCertificatesByOwnerAddressQuery, IEnumerable<VaultCertificate>>, RetrieveVaultCertificatesByOwnerAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusBlockHashByHeightQuery, string>, RetrieveCirrusBlockHashByHeightQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMarketByIdQuery, Market>, RetrieveMarketByIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>, RetrieveAddressStakingByLiquidityPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>, RetrieveAddressMiningByMiningPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQuery, AddressBalance>, RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAddressBalanceByTokenIdAndOwnerQuery, AddressBalance>, RetrieveAddressBalanceByTokenIdAndOwnerQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMiningPoolByAddressQuery, MiningPool>, RetrieveMiningPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveActiveMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceNomination>>, RetrieveActiveMiningGovernanceNominationsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceNominationCirrusDto>>, RetrieveCirrusMiningGovernanceNominationsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByIdQuery, LiquidityPool>, RetrieveLiquidityPoolByIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusLocalCallSmartContractQuery, LocalCallResponseDto>, RetrieveCirrusLocalCallSmartContractQueryHandler>();

            // Entry Commands
            services.AddTransient<IRequestHandler<CreateWalletSwapTransactionCommand, string>, CreateWalletSwapTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletAddLiquidityTransactionCommand, string>, CreateWalletAddLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletRemoveLiquidityTransactionCommand, string>, CreateWalletRemoveLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletApproveAllowanceTransactionCommand, string>, CreateWalletApproveAllowanceTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletCreateLiquidityPoolTransactionCommand, string>, CreateWalletCreateLiquidityPoolTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateTransactionCommand, bool>, CreateTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateCrsTokenSnapshotsCommand, Unit>, CreateCrsTokenSnapshotsCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletSkimTransactionCommand, string>, CreateWalletSkimTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletSyncTransactionCommand, string>, CreateWalletSyncTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletStartStakingTransactionCommand, string>, CreateWalletStartStakingTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletStopStakingTransactionCommand, string>, CreateWalletStopStakingTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletCollectStakingRewardsTransactionCommand, string>, CreateWalletCollectStakingRewardsTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletStartMiningTransactionCommand, string>, CreateWalletStartMiningTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletStopMiningTransactionCommand, string>, CreateWalletStopMiningTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletCollectMiningRewardsTransactionCommand, string>, CreateWalletCollectMiningRewardsTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletDistributeTokensTransactionCommand, string>, CreateWalletDistributeTokensTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletRewardMiningPoolsTransactionCommand, string>, CreateWalletRewardMiningPoolsTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessOdxDeploymentTransactionCommand, Unit>, ProcessOdxDeploymentTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessDeployerDeploymentTransactionCommand, Unit>, ProcessDeployerDeploymentTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessCreateLiquidityPoolLogCommand, bool>, ProcessCreateLiquidityPoolLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessApprovalLogCommand, bool>, ProcessApprovalLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessTransferLogCommand, bool>, ProcessTransferLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessCreateMarketLogCommand, bool>, ProcessCreateMarketLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessDistributionLogCommand, bool>, ProcessDistributionLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessChangeVaultOwnerLogCommand, bool>, ProcessChangeVaultOwnerLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessChangeMarketOwnerLogCommand, bool>, ProcessChangeMarketOwnerLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessMineLogCommand, bool>, ProcessMineLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessNominationLogCommand, bool>, ProcessNominationLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessChangeMarketPermissionLogCommand, bool>, ProcessChangeMarketPermissionLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessMintLogCommand, bool>, ProcessMintLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessBurnLogCommand, bool>, ProcessBurnLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessSwapLogCommand, bool>, ProcessSwapLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessReservesLogCommand, bool>, ProcessReservesLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessChangeMarketLogCommand, bool>, ProcessChangeMarketLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessCollectMiningRewardsLogCommand, bool>, ProcessCollectMiningRewardsLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessEnableMiningLogCommand, bool>, ProcessEnableMiningLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessStartStakingLogCommand, bool>, ProcessStartStakingLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessStopStakingLogCommand, bool>, ProcessStopStakingLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessCollectStakingRewardsLogCommand, bool>, ProcessCollectStakingRewardsLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessRewardMiningPoolLogCommand, bool>, ProcessRewardMiningPoolLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessCreateVaultCertificateLogCommand, bool>, ProcessCreateVaultCertificateLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessRevokeVaultCertificateLogCommand, bool>, ProcessRevokeVaultCertificateLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessRedeemVaultCertificateLogCommand, bool>, ProcessRedeemVaultCertificateLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessChangeDeployerOwnerLogCommand, bool>, ProcessChangeDeployerOwnerLogCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessLiquidityPoolSnapshotsByTransactionCommand, Unit>, ProcessLiquidityPoolSnapshotsByTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateBlockCommand, bool>, CreateBlockCommandHandler>();
            services.AddTransient<IRequestHandler<ProcessLatestBlocksCommand, Unit>, ProcessLatestBlocksCommandHandler>();

            // Commands
            services.AddTransient<IRequestHandler<MakeWalletSwapTransactionCommand, string>, MakeWalletSwapTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletAddLiquidityTransactionCommand, string>, MakeWalletAddLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletRemoveLiquidityTransactionCommand, string>, MakeWalletRemoveLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletApproveAllowanceTransactionCommand, string>, MakeWalletApproveAllowanceTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletCreateLiquidityPoolTransactionCommand, string>, MakeWalletCreateLiquidityPoolTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenCommand, long>, MakeTokenCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionCommand, long>, MakeTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeLiquidityPoolCommand, long>, MakeLiquidityPoolCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMiningPoolCommand, long>, MakeMiningPoolCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenSnapshotCommand, bool>, MakeTokenSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMarketCommand, long>, MakeMarketCommandHandler>();
            services.AddTransient<IRequestHandler<MakeLiquidityPoolSnapshotCommand, bool>, MakeLiquidityPoolSnapshotCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletSkimTransactionCommand, string>, MakeWalletSkimTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletSyncTransactionCommand, string>, MakeWalletSyncTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletStartStakingTransactionCommand, string>, MakeWalletStartStakingTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletStopStakingTransactionCommand, string>, MakeWalletStopStakingTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletCollectStakingRewardsTransactionCommand, string>, MakeWalletCollectStakingRewardsTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletStartMiningTransactionCommand, string>, MakeWalletStartMiningTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletStopMiningTransactionCommand, string>, MakeWalletStopMiningTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletCollectMiningRewardsTransactionCommand, string>, MakeWalletCollectMiningRewardsTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletDistributeTokensTransactionCommand, string>, MakeWalletDistributeTokensTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletRewardMiningPoolsTransactionCommand, string>, MakeWalletRewardMiningPoolsTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeDeployerCommand, long>, MakeDeployerCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMiningGovernanceCommand, long>, MakeMiningGovernanceCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenDistributionCommand, bool>, MakeTokenDistributionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeVaultCommand, long>, MakeVaultCommandHandler>();
            services.AddTransient<IRequestHandler<MakeVaultCertificateCommand, bool>, MakeVaultCertificateCommandHandler>();
            services.AddTransient<IRequestHandler<MakeAddressBalanceCommand, long>, MakeAddressBalanceCommandHandler>();
            services.AddTransient<IRequestHandler<MakeAddressAllowanceCommand, long>, MakeAddressAllowanceCommandHandler>();
            services.AddTransient<IRequestHandler<MakeAddressStakingCommand, long>, MakeAddressStakingCommandHandler>();
            services.AddTransient<IRequestHandler<MakeAddressMiningCommand, long>, MakeAddressMiningCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMiningGovernanceNominationCommand, long>, MakeMiningGovernanceNominationCommandHandler>();

            // Entry Handlers
            services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, BlockDto>, RetrieveLatestBlockQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenByAddressQuery, TokenDto>, GetTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolByAddressQuery, LiquidityPoolDto>, GetLiquidityPoolByAddressQueryHandler>();

            // Handlers
            services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByAddressQuery, LiquidityPool>, RetrieveLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionByHashQuery, Transaction>, RetrieveTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, RetrieveTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTokenByIdQuery, Token>, RetrieveTokenByIdQueryHandler>();

            // Assemblers
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            services.AddTransient<IModelAssembler<LiquidityPool, LiquidityPoolDto>, LiquidityPoolDtoAssembler>();

            return services;
        }
    }
}