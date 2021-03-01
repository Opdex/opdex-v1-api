using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pairs;
using Opdex.Platform.Application.Abstractions.Queries.Pairs;

namespace Opdex.Platform.Application.EntryHandlers.Pairs
{
    public class GetAllPairsQueryHandler : IRequestHandler<GetAllPairsQuery, IEnumerable<PairDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public GetAllPairsQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PairDto>> Handle(GetAllPairsQuery request, CancellationToken cancellationToken)
        {
            var query = new RetrieveAllPairsQuery();
            
            var pairs = await _mediator.Send(query, cancellationToken);
            
            return _mapper.Map<IEnumerable<PairDto>>(pairs);
        }
    }
}