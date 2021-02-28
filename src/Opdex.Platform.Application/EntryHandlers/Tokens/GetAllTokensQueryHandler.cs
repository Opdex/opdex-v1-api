using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetAllTokensQueryHandler : IRequestHandler<GetAllTokensQuery, IEnumerable<TokenDto>>
    {
        private readonly IMediator _mediator;
        
        public GetAllTokensQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<TokenDto>> Handle(GetAllTokensQuery request, CancellationToken cancellationToken)
        {
            var query = new RetrieveAllTokensQuery();
            
            var tokens = await _mediator.Send(query, cancellationToken);

            return tokens;
        }
    }
}