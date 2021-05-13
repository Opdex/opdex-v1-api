using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
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