using System;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application.Assemblers
{
    public class TransactionDtoAssembler : IModelAssembler<Transaction, TransactionDto>
    {
        private readonly IMediator _mediator;
        
        public TransactionDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<TransactionDto> Assemble(Transaction source)
        {
            throw new System.NotImplementedException();
        }
    }
}