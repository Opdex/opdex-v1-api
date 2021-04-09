using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Core.Domain.Models;

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
            var transactionLogs = await _mediator.Send(new RetrieveTransactionLogsByTransactionIdQuery(transaction.Id));
            
            return _mapper.Map<TransactionDto>(new Transaction(transaction.Id, transaction.Hash, transaction.BlockHeight,
                transaction.GasUsed, transaction.From, transaction.To, transactionLogs));
        }
    }
}