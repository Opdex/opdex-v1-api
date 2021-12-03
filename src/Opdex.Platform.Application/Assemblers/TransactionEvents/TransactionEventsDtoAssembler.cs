using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.VaultGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents;

public class TransactionEventsDtoAssembler : IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>>
{
    private readonly IMapper _mapper;
    private readonly IModelAssembler<SwapLog, SwapEventDto> _swapEventDtoAssembler;
    private readonly IModelAssembler<MintLog, AddLiquidityEventDto> _mintProvideEventDtoAssembler;
    private readonly IModelAssembler<BurnLog, RemoveLiquidityEventDto> _burnProvideEventDtoAssembler;
    private readonly IModelAssembler<TransferLog, TransferEventDto> _transferEventDtoAssembler;
    private readonly IModelAssembler<ApprovalLog, ApprovalEventDto> _approvalEventDtoAssembler;

    public TransactionEventsDtoAssembler(IMapper mapper,
                                         IModelAssembler<SwapLog, SwapEventDto> swapEventDtoAssembler,
                                         IModelAssembler<MintLog, AddLiquidityEventDto> mintLogDtoAssembler,
                                         IModelAssembler<BurnLog, RemoveLiquidityEventDto> burnLogDtoAssembler,
                                         IModelAssembler<TransferLog, TransferEventDto> transferEventDtoAssembler,
                                         IModelAssembler<ApprovalLog, ApprovalEventDto> approvalEventDtoAssembler)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _swapEventDtoAssembler = swapEventDtoAssembler ?? throw new ArgumentNullException(nameof(swapEventDtoAssembler));
        _mintProvideEventDtoAssembler = mintLogDtoAssembler ?? throw new ArgumentNullException(nameof(mintLogDtoAssembler));
        _burnProvideEventDtoAssembler = burnLogDtoAssembler ?? throw new ArgumentNullException(nameof(burnLogDtoAssembler));
        _transferEventDtoAssembler = transferEventDtoAssembler ?? throw new ArgumentNullException(nameof(transferEventDtoAssembler));
        _approvalEventDtoAssembler = approvalEventDtoAssembler ?? throw new ArgumentNullException(nameof(approvalEventDtoAssembler));
    }

    public async Task<IReadOnlyCollection<TransactionEventDto>> Assemble(IEnumerable<TransactionLog> logs)
    {
        var events = new List<TransactionEventDto>();

        foreach (var log in logs)
        {
            TransactionEventDto logDto = log.LogType switch
            {
                // Deployers
                TransactionLogType.ClaimPendingDeployerOwnershipLog => _mapper.Map<ClaimPendingDeployerOwnershipEventDto>((ClaimPendingDeployerOwnershipLog)log),
                TransactionLogType.SetPendingDeployerOwnershipLog => _mapper.Map<SetPendingDeployerOwnershipEventDto>((SetPendingDeployerOwnershipLog)log),
                TransactionLogType.CreateMarketLog => _mapper.Map<CreateMarketEventDto>((CreateMarketLog)log),

                // Markets
                TransactionLogType.ClaimPendingMarketOwnershipLog => _mapper.Map<ClaimPendingMarketOwnershipEventDto>((ClaimPendingMarketOwnershipLog)log),
                TransactionLogType.SetPendingMarketOwnershipLog => _mapper.Map<SetPendingMarketOwnershipEventDto>((SetPendingMarketOwnershipLog)log),
                TransactionLogType.ChangeMarketPermissionLog => _mapper.Map<ChangeMarketPermissionEventDto>((ChangeMarketPermissionLog)log),
                TransactionLogType.CreateLiquidityPoolLog => _mapper.Map<CreateLiquidityPoolEventDto>((CreateLiquidityPoolLog)log),

                // Liquidity Pools
                TransactionLogType.SwapLog => await _swapEventDtoAssembler.Assemble((SwapLog)log),
                TransactionLogType.MintLog => await _mintProvideEventDtoAssembler.Assemble((MintLog)log),
                TransactionLogType.BurnLog => await _burnProvideEventDtoAssembler.Assemble((BurnLog)log),
                TransactionLogType.StartStakingLog => _mapper.Map<StartStakingEventDto>((StartStakingLog)log),
                TransactionLogType.StopStakingLog => _mapper.Map<StopStakingEventDto>((StopStakingLog)log),
                TransactionLogType.CollectStakingRewardsLog => _mapper.Map<CollectStakingRewardsEventDto>((CollectStakingRewardsLog)log),

                // Mining Pools
                TransactionLogType.EnableMiningLog => _mapper.Map<EnableMiningEventDto>((EnableMiningLog)log),
                TransactionLogType.StartMiningLog => _mapper.Map<StartMiningEventDto>((StartMiningLog)log),
                TransactionLogType.StopMiningLog => _mapper.Map<StopMiningEventDto>((StopMiningLog)log),
                TransactionLogType.CollectMiningRewardsLog => _mapper.Map<CollectMiningRewardsEventDto>((CollectMiningRewardsLog)log),

                // Tokens
                TransactionLogType.TransferLog => await _transferEventDtoAssembler.Assemble((TransferLog)log),
                TransactionLogType.ApprovalLog => await _approvalEventDtoAssembler.Assemble((ApprovalLog)log),
                TransactionLogType.DistributionLog => _mapper.Map<DistributionEventDto>((DistributionLog)log),

                // Mining Governances
                TransactionLogType.NominationLog => _mapper.Map<NominationEventDto>((NominationLog)log),
                TransactionLogType.RewardMiningPoolLog => _mapper.Map<RewardMiningPoolEventDto>((RewardMiningPoolLog)log),

                // Vaults
                TransactionLogType.ClaimPendingVaultOwnershipLog => _mapper.Map<ClaimPendingVaultOwnershipEventDto>((ClaimPendingVaultOwnershipLog)log),
                TransactionLogType.SetPendingVaultOwnershipLog => _mapper.Map<SetPendingVaultOwnershipEventDto>((SetPendingVaultOwnershipLog)log),
                TransactionLogType.CreateVaultCertificateLog => _mapper.Map<CreateVaultCertificateEventDto>((CreateVaultCertificateLog)log),
                TransactionLogType.RedeemVaultCertificateLog => _mapper.Map<RedeemVaultCertificateEventDto>((RedeemVaultCertificateLog)log),
                TransactionLogType.RevokeVaultCertificateLog => _mapper.Map<RevokeVaultCertificateEventDto>((RevokeVaultCertificateLog)log),

                // Vault Governance
                TransactionLogType.CreateVaultProposalLog => _mapper.Map<CreateVaultProposalEventDto>((CreateVaultProposalLog)log),
                TransactionLogType.CompleteVaultProposalLog => _mapper.Map<CompleteVaultProposalEventDto>((CompleteVaultProposalLog)log),
                TransactionLogType.VaultProposalPledgeLog => _mapper.Map<VaultProposalPledgeEventDto>((VaultProposalPledgeLog)log),
                TransactionLogType.VaultProposalWithdrawPledgeLog => _mapper.Map<VaultProposalWithdrawPledgeEventDto>((VaultProposalWithdrawPledgeLog)log),
                TransactionLogType.VaultProposalVoteLog => _mapper.Map<VaultProposalVoteEventDto>((VaultProposalVoteLog)log),
                TransactionLogType.VaultProposalWithdrawVoteLog => _mapper.Map<VaultProposalWithdrawVoteEventDto>((VaultProposalWithdrawVoteLog)log),
                _ => null
            };

            if (logDto != null)
            {
                events.Add(logDto);
            }
        }

        return events.OrderBy(log => log.SortOrder).ToList();
    }
}
