using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;

namespace Opdex.Core.Application.Assemblers
{
    public class TransactionDtoAssembler : IModelAssembler<Transaction, TransactionDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public TransactionDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<TransactionDto> Assemble(Transaction transaction)
        {
            var transactionLogs = new List<TransactionLog>();

            var request = new RetrieveTransactionLogSummariesByTransactionIdQuery(transaction.Id);
            
            var summaries = await _mediator.Send(request);

            foreach (var group in summaries.GroupBy(s => s.LogTypeId))
            {
                var logType = (TransactionLogType)group.Key;
                
                var groupLogs = await GetLogs(logType, group);
                
                transactionLogs.AddRange(groupLogs);
            }

            return _mapper.Map<TransactionDto>(new Transaction(transaction.Id, transaction.Hash, transaction.BlockHeight,
                transaction.GasUsed, transaction.From, transaction.To, transactionLogs));
        }

        private async Task<IEnumerable<TransactionLog>> GetLogs(TransactionLogType logType, IEnumerable<TransactionLogSummary> txLogs)
        {
            return logType switch
            {
                TransactionLogType.LiquidityPoolCreatedLog => await _mediator.Send(new RetrieveLiquidityPoolCreatedLogsByIdsQuery(txLogs)),
                TransactionLogType.BurnLog => await _mediator.Send(new RetrieveBurnLogsByIdsQuery(txLogs)),
                TransactionLogType.MintLog => await _mediator.Send(new RetrieveMintLogsByIdsQuery(txLogs)),
                TransactionLogType.SwapLog => await _mediator.Send(new RetrieveSwapLogsByIdsQuery(txLogs)),
                TransactionLogType.ReservesLog => await _mediator.Send(new RetrieveReservesLogsByIdsQuery(txLogs)),
                TransactionLogType.ApprovalLog => await _mediator.Send(new RetrieveApprovalLogsByIdsQuery(txLogs)),
                TransactionLogType.TransferLog => await _mediator.Send(new RetrieveTransferLogsByIdsQuery(txLogs)),
                TransactionLogType.EnterStakingPoolLog => null,
                TransactionLogType.CollectStakingRewardsLog => null,
                TransactionLogType.ExitStakingPoolLog => null,
                TransactionLogType.EnterMiningPoolLog => null,
                TransactionLogType.CollectMiningRewardsLog => null,
                TransactionLogType.ExitMiningPoolLog => null,
                TransactionLogType.MiningPoolCreatedLog => null,
                TransactionLogType.RewardMiningPoolLog => null,
                TransactionLogType.NominationLog => null,
                TransactionLogType.MiningPoolRewardedLog => null,
                TransactionLogType.OwnerChangeLog => null,
                TransactionLogType.DistributionLog => null,
                _ => null
            };
        }
    }
}