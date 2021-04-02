using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;

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
            var transactionEvents = new List<TransactionEvent>();

            var request = new RetrieveTransactionEventSummariesByTransactionIdQuery(transaction.Id);
            
            var summaries = await _mediator.Send(request);

            foreach (var group in summaries.GroupBy(s => s.EventTypeId))
            {
                var eventType = (TransactionEventType)group.Key;
                
                var groupEvents = await GetEvents(eventType, group);
                
                transactionEvents.AddRange(groupEvents);
            }

            return _mapper.Map<TransactionDto>(new Transaction(transaction.Id, transaction.Hash, transaction.BlockHeight,
                transaction.GasUsed, transaction.From, transaction.To, transactionEvents));
        }

        private async Task<IEnumerable<TransactionEvent>> GetEvents(TransactionEventType eventType, IEnumerable<TransactionEventSummary> txEvents)
        {
            return eventType switch
            {
                TransactionEventType.PairCreatedEvent => await _mediator.Send(new RetrievePairCreatedEventsByTransactionIdQuery(txEvents)),
                TransactionEventType.BurnEvent => await _mediator.Send(new RetrieveBurnEventsByTransactionIdQuery(txEvents)),
                TransactionEventType.MintEvent => await _mediator.Send(new RetrieveMintEventsByTransactionIdQuery(txEvents)),
                TransactionEventType.SwapEvent => await _mediator.Send(new RetrieveSwapEventsByTransactionIdQuery(txEvents)),
                TransactionEventType.SyncEvent => await _mediator.Send(new RetrieveSyncEventsByTransactionIdQuery(txEvents)),
                TransactionEventType.ApprovalEvent => await _mediator.Send(new RetrieveApprovalEventsByTransactionIdQuery(txEvents)),
                TransactionEventType.TransferEvent => await _mediator.Send(new RetrieveTransferEventsByTransactionIdQuery(txEvents)),
                _ => null
            };
        }
    }
}