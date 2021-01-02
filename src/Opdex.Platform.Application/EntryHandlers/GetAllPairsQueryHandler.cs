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
    public class GetAllPairsQueryHandler : IRequestHandler<GetAllPairsQuery, IEnumerable<PairDto>>
    {
        private readonly IMediator _mediator;
        
        public GetAllPairsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<PairDto>> Handle(GetAllPairsQuery request, CancellationToken cancellationToken)
        {
            var query = new RetrieveAllPairsQuery();
            
            var pairs = await _mediator.Send(query, cancellationToken);
            
            return pairs;
        }
    }
}