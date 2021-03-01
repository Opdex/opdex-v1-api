using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Tokens;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Core.Application.Handlers.Tokens
{
    public class RetrieveTokenByAddressQueryHandler : IRequestHandler<RetrieveTokenByAddressQuery, Token>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTokenByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<Token> Handle(RetrieveTokenByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new SelectTokenByAddressQuery(request.Address), cancellationToken);

            return token;
        }
    }
}