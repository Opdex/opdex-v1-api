using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
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