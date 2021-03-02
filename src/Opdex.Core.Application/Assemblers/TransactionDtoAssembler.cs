using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;

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
            
            var burnEvents = await _mediator.Send(new RetrieveBurnEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(burnEvents);
            
            var mintEvents = await _mediator.Send(new RetrieveMintEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(mintEvents);

            var swapEvents = await _mediator.Send(new RetrieveSwapEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(swapEvents);

            var syncEvents = await _mediator.Send(new RetrieveSyncEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(syncEvents);

            var approvalEvents = await _mediator.Send(new RetrieveApprovalEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(approvalEvents);

            var transferEvents = await _mediator.Send(new RetrieveTransferEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(transferEvents);

            var pairsCreated = await _mediator.Send(new RetrievePairCreatedEventsByTransactionIdQuery(transaction.Id));
            transactionEvents.AddRange(pairsCreated);

            var fullTransaction = new Transaction(transaction.Id, transaction.Hash, transaction.BlockHeight,
                transaction.GasUsed, transaction.From, transaction.To, transactionEvents);

            return _mapper.Map<TransactionDto>(fullTransaction);
        }
    }
}