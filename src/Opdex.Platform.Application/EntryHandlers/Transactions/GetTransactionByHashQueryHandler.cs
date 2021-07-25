using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class GetTransactionByHashQueryHandler : IRequestHandler<GetTransactionByHashQuery, TransactionDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Transaction, TransactionDto> _transactionDtoAssembler;

        public GetTransactionByHashQueryHandler(IMediator mediator, IModelAssembler<Transaction, TransactionDto> transactionDtoAssembler)
        {
            _mediator = mediator;
            _transactionDtoAssembler = transactionDtoAssembler;
        }

        public async Task<TransactionDto> Handle(GetTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new RetrieveTransactionByHashQuery(request.Hash), cancellationToken);
            return await _transactionDtoAssembler.Assemble(vault);
        }
    }
}
