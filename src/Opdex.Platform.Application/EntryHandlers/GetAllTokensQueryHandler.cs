using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries;
using Opdex.Platform.Application.Abstractions.Queries;

namespace Opdex.Platform.Application.EntryHandlers
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