using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class GetTransactionsByPoolWithFilterQueryHandler 
        : IRequestHandler<GetTransactionsByPoolWithFilterQuery, IEnumerable<TransactionDto>>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Transaction, TransactionDto> _assembler;
        
        public GetTransactionsByPoolWithFilterQueryHandler(IMediator mediator, IModelAssembler<Transaction, TransactionDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }


        public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionsByPoolWithFilterQuery request, CancellationToken cancellationToken)
        {
            var query = new RetrieveTransactionsByPoolWithFilterQuery(request.PoolAddress, request.LogTypes);

            var transactions = await _mediator.Send(query, cancellationToken);

            var transactionDtos = new List<TransactionDto>();
            
            foreach (var transaction in transactions)
            {
                var transactionDto = await _assembler.Assemble(transaction);
                transactionDtos.Add(transactionDto);
            }

            return transactionDtos;
        }
    }
}