using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Pairs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Platform.Application.Handlers.Pairs
{
    public class RetrieveAllPairsQueryHandler : IRequestHandler<RetrieveAllPairsQuery, IEnumerable<PairDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveAllPairsQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PairDto>> Handle(RetrieveAllPairsQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAllPairsQuery();
            
            var pairs = await _mediator.Send(query, cancellationToken);
            
            return _mapper.Map<IEnumerable<PairDto>>(pairs);
        }
    }
}