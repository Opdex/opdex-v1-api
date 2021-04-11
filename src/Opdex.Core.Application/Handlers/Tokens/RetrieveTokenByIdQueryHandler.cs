using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Tokens;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Core.Application.Handlers.Tokens
{
    public class RetrieveTokenByIdQueryHandler : IRequestHandler<RetrieveTokenByIdQuery, Token>
    {
        private readonly IMediator _mediator;

        public RetrieveTokenByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<Token> Handle(RetrieveTokenByIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectTokenByIdQuery(request.TokenId));
        }
    }
}