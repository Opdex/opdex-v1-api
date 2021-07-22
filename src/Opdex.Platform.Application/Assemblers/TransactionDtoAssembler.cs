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
        private readonly IModelAssembler<MintLog, ProvideEventDto> _mintProvideEventDtoAssembler;
        private readonly IModelAssembler<BurnLog, ProvideEventDto> _burnProvideEventDtoAssembler;
        private readonly IModelAssembler<TransferLog, TransferEventDto> _transferEventDtoAssembler;
        private readonly IModelAssembler<ApprovalLog, ApprovalEventDto> _approvalEventDtoAssembler;

        public TransactionDtoAssembler(IMediator mediator,
                                       IMapper mapper,
                                       IModelAssembler<SwapLog, SwapEventDto> swapEventDtoAssembler,
                                       IModelAssembler<MintLog, ProvideEventDto> mintLogDtoAssembler,
                                       IModelAssembler<BurnLog, ProvideEventDto> burnLogDtoAssembler,
                                       IModelAssembler<TransferLog, TransferEventDto> transferEventDtoAssembler,
                                       IModelAssembler<ApprovalLog, ApprovalEventDto> approvalEventDtoAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _swapEventDtoAssembler = swapEventDtoAssembler ?? throw new ArgumentNullException(nameof(swapEventDtoAssembler));
            _mintProvideEventDtoAssembler = mintLogDtoAssembler ?? throw new ArgumentNullException(nameof(mintLogDtoAssembler));
            _burnProvideEventDtoAssembler = burnLogDtoAssembler ?? throw new ArgumentNullException(nameof(burnLogDtoAssembler));
            _transferEventDtoAssembler = transferEventDtoAssembler ?? throw new ArgumentNullException(nameof(transferEventDtoAssembler));
            _approvalEventDtoAssembler = approvalEventDtoAssembler ?? throw new ArgumentNullException(nameof(approvalEventDtoAssembler));
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
                    TransactionLogType.ChangeDeployerOwnerLog => _mapper.Map<ChangeDeployerOwnerEventDto>((ChangeDeployerOwnerLog)log),
                    TransactionLogType.CreateMarketLog => _mapper.Map<CreateMarketEventDto>((CreateMarketLog)log),

                    // Markets
                    TransactionLogType.ChangeMarketOwnerLog => _mapper.Map<ChangeMarketOwnerEventDto>((ChangeMarketOwnerLog)log),
                    TransactionLogType.ChangeMarketPermissionLog => _mapper.Map<ChangeMarketOwnerEventDto>((ChangeMarketPermissionLog)log),
                    TransactionLogType.CreateLiquidityPoolLog => _mapper.Map<CreateLiquidityPoolEventDto>((CreateLiquidityPoolLog)log),

                    // Liquidity Pools
                    TransactionLogType.SwapLog => await _swapEventDtoAssembler.Assemble((SwapLog)log),
                    TransactionLogType.MintLog => await _mintProvideEventDtoAssembler.Assemble((MintLog)log),
                    TransactionLogType.BurnLog => await _burnProvideEventDtoAssembler.Assemble((BurnLog)log),
                    TransactionLogType.StakeLog => _mapper.Map<StakeEventDto>((StakeLog)log),
                    TransactionLogType.CollectStakingRewardsLog => _mapper.Map<CollectStakingRewardsEventDto>((CollectStakingRewardsLog)log),

                    // Mining Pools
                    TransactionLogType.EnableMiningLog => _mapper.Map<EnableMiningEventDto>((EnableMiningLog)log),
                    TransactionLogType.MineLog => _mapper.Map<MineEventDto>((MineLog)log),
                    TransactionLogType.CollectMiningRewardsLog => _mapper.Map<CollectMiningRewardsEventDto>((CollectMiningRewardsLog)log),

                    // Tokens
                    TransactionLogType.TransferLog => await _transferEventDtoAssembler.Assemble((TransferLog)log),
                    TransactionLogType.ApprovalLog => await _approvalEventDtoAssembler.Assemble((ApprovalLog)log),
                    TransactionLogType.DistributionLog => _mapper.Map<DistributionEventDto>((DistributionLog)log),

                    // Governances
                    TransactionLogType.NominationLog => _mapper.Map<NominationEventDto>((NominationLog)log),
                    TransactionLogType.RewardMiningPoolLog => _mapper.Map<RewardMiningPoolEventDto>((RewardMiningPoolLog)log),

                    // Vaults
                    TransactionLogType.ChangeVaultOwnerLog => _mapper.Map<ChangeVaultOwnerEventDto>((ChangeVaultOwnerLog)log),
                    TransactionLogType.CreateVaultCertificateLog => _mapper.Map<CreateVaultCertificateEventDto>((CreateVaultCertificateLog)log),
                    TransactionLogType.RedeemVaultCertificateLog => _mapper.Map<RedeemVaultCertificateEventDto>((RedeemVaultCertificateLog)log),
                    TransactionLogType.RevokeVaultCertificateLog => _mapper.Map<RevokeVaultCertificateEventDto>((RevokeVaultCertificateLog)log),
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
