using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Assemblers
{
    public class TransactionDtoAssembler : IModelAssembler<Transaction, TransactionDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<SwapLog, SwapEventDto> _swapEventDtoAssembler;
        private readonly IModelAssembler<MintLog, ProvideEventDto> _mintLogDtoAssembler;
        private readonly IModelAssembler<BurnLog, ProvideEventDto> _burnLogDtoAssembler;
        private readonly IModelAssembler<EnableMiningLog, EnableMiningEventDto> _enableMiningDtoAssembler;
        private readonly IModelAssembler<StakeLog, StakeEventDto> _stakeEventDtoAssembler;
        private readonly IModelAssembler<MineLog, MineEventDto> _mineEventDtoAssembler;
        private readonly IModelAssembler<CollectMiningRewardsLog, CollectMiningRewardsEventDto> _collectMiningRewardsEventDtoAssembler;
        private readonly IModelAssembler<CollectStakingRewardsLog, CollectStakingRewardsEventDto> _collectStakingRewardsEventDtoAssembler;
        private readonly IModelAssembler<ChangeMarketOwnerLog, ChangeMarketOwnerEventDto> _changeMarketOwnerEventDtoAssembler;
        private readonly IModelAssembler<ChangeVaultOwnerLog, ChangeVaultOwnerEventDto> _changeVaultOwnerEventDtoAssembler;
        private readonly IModelAssembler<CreateVaultCertificateLog, CreateVaultCertificateEventDto> _createVaultCertificateEventDtoAssembler;
        private readonly IModelAssembler<RedeemVaultCertificateLog, RedeemVaultCertificateEventDto> _redeemVaultCertificateEventDtoAssembler;
        private readonly IModelAssembler<RevokeVaultCertificateLog, RevokeVaultCertificateEventDto> _revokeVaultCertificateEventDtoAssembler;
        private readonly IModelAssembler<ChangeDeployerOwnerLog, ChangeDeployerOwnerEventDto> _changeDeployerOwnerEventDtoAssembler;
        private readonly IModelAssembler<TransferLog, TransferEventDto> _transferEventDtoAssembler;
        private readonly IModelAssembler<ApprovalLog, ApprovalEventDto> _approvalEventDtoAssembler;
        private readonly IModelAssembler<DistributionLog, DistributionEventDto> _distributionEventDtoAssembler;
        private readonly IModelAssembler<NominationLog, NominationEventDto> _nominationEventDtoAssembler;
        private readonly IModelAssembler<RewardMiningPoolLog, RewardMiningPoolEventDto> _rewardMiningPoolEventDtoAssembler;
        private readonly IModelAssembler<CreateMarketLog, CreateMarketEventDto> _createMarketEventDtoAssembler;
        private readonly IModelAssembler<ChangeMarketPermissionLog, ChangeMarketPermissionEventDto> _changeMarketPermissionEventDtoAssembler;
        private readonly IModelAssembler<CreateLiquidityPoolLog, CreateLiquidityPoolEventDto> _createLiquidityPoolEventDtoAssembler;

        public TransactionDtoAssembler(IMediator mediator,
                                       IMapper mapper,
                                       IModelAssembler<SwapLog, SwapEventDto> swapEventDtoAssembler,
                                       IModelAssembler<MintLog, ProvideEventDto> mintLogDtoAssembler,
                                       IModelAssembler<BurnLog, ProvideEventDto> burnLogDtoAssembler,
                                       IModelAssembler<EnableMiningLog, EnableMiningEventDto> enableMiningDtoAssembler,
                                       IModelAssembler<StakeLog, StakeEventDto> stakeEventDtoAssembler,
                                       IModelAssembler<MineLog, MineEventDto> mineEventDtoAssembler,
                                       IModelAssembler<CollectMiningRewardsLog, CollectMiningRewardsEventDto> collectMiningRewardsEventDtoAssembler,
                                       IModelAssembler<CollectStakingRewardsLog, CollectStakingRewardsEventDto> collectStakingRewardsEventDtoAssembler,
                                       IModelAssembler<ChangeMarketOwnerLog, ChangeMarketOwnerEventDto> changeMarketOwnerEventDtoAssembler,
                                       IModelAssembler<ChangeVaultOwnerLog, ChangeVaultOwnerEventDto> changeVaultOwnerEventDtoAssembler,
                                       IModelAssembler<CreateVaultCertificateLog, CreateVaultCertificateEventDto> createVaultCertificateEventDtoAssembler,
                                       IModelAssembler<RedeemVaultCertificateLog, RedeemVaultCertificateEventDto> redeemVaultCertificateEventDtoAssembler,
                                       IModelAssembler<RevokeVaultCertificateLog, RevokeVaultCertificateEventDto> revokeVaultCertificateEventDtoAssembler,
                                       IModelAssembler<ChangeDeployerOwnerLog, ChangeDeployerOwnerEventDto> changeDeployerOwnerEventDtoAssembler,
                                       IModelAssembler<TransferLog, TransferEventDto> transferEventDtoAssembler,
                                       IModelAssembler<ApprovalLog, ApprovalEventDto> approvalEventDtoAssembler,
                                       IModelAssembler<DistributionLog, DistributionEventDto> distributionEventDtoAssembler,
                                       IModelAssembler<NominationLog, NominationEventDto> nominationEventDtoAssembler,
                                       IModelAssembler<RewardMiningPoolLog, RewardMiningPoolEventDto> rewardMiningPoolEventDtoAssembler,
                                       IModelAssembler<CreateMarketLog, CreateMarketEventDto> createMarketEventDtoAssembler,
                                       IModelAssembler<ChangeMarketPermissionLog, ChangeMarketPermissionEventDto> changeMarketPermissionEventDtoAssembler,
                                       IModelAssembler<CreateLiquidityPoolLog, CreateLiquidityPoolEventDto> createLiquidityPoolEventDtoAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _swapEventDtoAssembler = swapEventDtoAssembler ?? throw new ArgumentNullException(nameof(swapEventDtoAssembler));
            _mintLogDtoAssembler = mintLogDtoAssembler ?? throw new ArgumentNullException(nameof(mintLogDtoAssembler));
            _burnLogDtoAssembler = burnLogDtoAssembler ?? throw new ArgumentNullException(nameof(burnLogDtoAssembler));
            _enableMiningDtoAssembler = enableMiningDtoAssembler ?? throw new ArgumentNullException(nameof(enableMiningDtoAssembler));
            _stakeEventDtoAssembler = stakeEventDtoAssembler ?? throw new ArgumentNullException(nameof(stakeEventDtoAssembler));
            _mineEventDtoAssembler = mineEventDtoAssembler ?? throw new ArgumentNullException(nameof(mineEventDtoAssembler));
            _collectMiningRewardsEventDtoAssembler = collectMiningRewardsEventDtoAssembler ?? throw new ArgumentNullException(nameof(collectMiningRewardsEventDtoAssembler));
            _collectStakingRewardsEventDtoAssembler = collectStakingRewardsEventDtoAssembler ?? throw new ArgumentNullException(nameof(collectStakingRewardsEventDtoAssembler));
            _changeMarketOwnerEventDtoAssembler = changeMarketOwnerEventDtoAssembler ?? throw new ArgumentNullException(nameof(changeMarketOwnerEventDtoAssembler));
            _changeVaultOwnerEventDtoAssembler = changeVaultOwnerEventDtoAssembler ?? throw new ArgumentNullException(nameof(changeVaultOwnerEventDtoAssembler));
            _createVaultCertificateEventDtoAssembler = createVaultCertificateEventDtoAssembler ?? throw new ArgumentNullException(nameof(createVaultCertificateEventDtoAssembler));
            _redeemVaultCertificateEventDtoAssembler = redeemVaultCertificateEventDtoAssembler ?? throw new ArgumentNullException(nameof(redeemVaultCertificateEventDtoAssembler));
            _revokeVaultCertificateEventDtoAssembler = revokeVaultCertificateEventDtoAssembler ?? throw new ArgumentNullException(nameof(revokeVaultCertificateEventDtoAssembler));
            _changeDeployerOwnerEventDtoAssembler = changeDeployerOwnerEventDtoAssembler ?? throw new ArgumentNullException(nameof(changeDeployerOwnerEventDtoAssembler));
            _transferEventDtoAssembler = transferEventDtoAssembler ?? throw new ArgumentNullException(nameof(transferEventDtoAssembler));
            _approvalEventDtoAssembler = approvalEventDtoAssembler ?? throw new ArgumentNullException(nameof(approvalEventDtoAssembler));
            _distributionEventDtoAssembler = distributionEventDtoAssembler ?? throw new ArgumentNullException(nameof(distributionEventDtoAssembler));
            _nominationEventDtoAssembler = nominationEventDtoAssembler ?? throw new ArgumentNullException(nameof(nominationEventDtoAssembler));
            _rewardMiningPoolEventDtoAssembler = rewardMiningPoolEventDtoAssembler ?? throw new ArgumentNullException(nameof(rewardMiningPoolEventDtoAssembler));
            _createMarketEventDtoAssembler = createMarketEventDtoAssembler ?? throw new ArgumentNullException(nameof(createMarketEventDtoAssembler));
            _changeMarketPermissionEventDtoAssembler = changeMarketPermissionEventDtoAssembler ?? throw new ArgumentNullException(nameof(changeMarketPermissionEventDtoAssembler));
            _createLiquidityPoolEventDtoAssembler = createLiquidityPoolEventDtoAssembler ?? throw new ArgumentNullException(nameof(createLiquidityPoolEventDtoAssembler));
        }

        public async Task<TransactionDto> Assemble(Transaction tx)
        {
            var txLogs = await _mediator.Send(new RetrieveTransactionLogsByTransactionIdQuery(tx.Id));
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(tx.BlockHeight));

            var txLogDtos = new List<TransactionEventDto>();

            foreach (var log in txLogs)
            {
                TransactionEventDto logDto = log.LogType switch
                {
                    // Deployers
                    TransactionLogType.ChangeDeployerOwnerLog => await _changeDeployerOwnerEventDtoAssembler.Assemble((ChangeDeployerOwnerLog)log),
                    TransactionLogType.CreateMarketLog => await _createMarketEventDtoAssembler.Assemble((CreateMarketLog)log),

                    // Markets
                    TransactionLogType.ChangeMarketOwnerLog => await _changeMarketOwnerEventDtoAssembler.Assemble((ChangeMarketOwnerLog)log),
                    TransactionLogType.ChangeMarketPermissionLog => await _changeMarketPermissionEventDtoAssembler.Assemble((ChangeMarketPermissionLog)log),
                    TransactionLogType.CreateLiquidityPoolLog => await _createLiquidityPoolEventDtoAssembler.Assemble((CreateLiquidityPoolLog)log),

                    // Liquidity Pools
                    TransactionLogType.SwapLog => await _swapEventDtoAssembler.Assemble((SwapLog)log),
                    TransactionLogType.MintLog => await _mintLogDtoAssembler.Assemble((MintLog)log),
                    TransactionLogType.BurnLog => await _burnLogDtoAssembler.Assemble((BurnLog)log),
                    TransactionLogType.StakeLog => await _stakeEventDtoAssembler.Assemble((StakeLog)log),
                    TransactionLogType.CollectStakingRewardsLog => await _collectStakingRewardsEventDtoAssembler.Assemble((CollectStakingRewardsLog)log),

                    // Mining Pools
                    TransactionLogType.EnableMiningLog => await _enableMiningDtoAssembler.Assemble((EnableMiningLog)log),
                    TransactionLogType.MineLog => await _mineEventDtoAssembler.Assemble((MineLog)log),
                    TransactionLogType.CollectMiningRewardsLog => await _collectMiningRewardsEventDtoAssembler.Assemble((CollectMiningRewardsLog)log),

                    // Tokens
                    TransactionLogType.TransferLog => await _transferEventDtoAssembler.Assemble((TransferLog)log),
                    TransactionLogType.ApprovalLog => await _approvalEventDtoAssembler.Assemble((ApprovalLog)log),
                    TransactionLogType.DistributionLog => await _distributionEventDtoAssembler.Assemble((DistributionLog)log),

                    // Governances
                    TransactionLogType.NominationLog => await _nominationEventDtoAssembler.Assemble((NominationLog)log),
                    TransactionLogType.RewardMiningPoolLog => await _rewardMiningPoolEventDtoAssembler.Assemble((RewardMiningPoolLog)log),

                    // Vaults
                    TransactionLogType.ChangeVaultOwnerLog => await _changeVaultOwnerEventDtoAssembler.Assemble((ChangeVaultOwnerLog)log),
                    TransactionLogType.CreateVaultCertificateLog => await _createVaultCertificateEventDtoAssembler.Assemble((CreateVaultCertificateLog)log),
                    TransactionLogType.RedeemVaultCertificateLog => await _redeemVaultCertificateEventDtoAssembler.Assemble((RedeemVaultCertificateLog)log),
                    TransactionLogType.RevokeVaultCertificateLog => await _revokeVaultCertificateEventDtoAssembler.Assemble((RevokeVaultCertificateLog)log),
                    _ => null
                };

                if (logDto != null) txLogDtos.Add(logDto);
            }

            var transactionDto =  _mapper.Map<TransactionDto>(new Transaction(tx.Id, tx.Hash, tx.BlockHeight, tx.GasUsed, tx.From, tx.To,
                                                                              tx.Success, tx.NewContractAddress));

            transactionDto.Events = txLogDtos.OrderBy(log => log.SortOrder);
            transactionDto.BlockDto = _mapper.Map<BlockDto>(block);

            return transactionDto;
        }
    }
}
