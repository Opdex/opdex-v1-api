using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.Queries.Pairs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Platform.Application.Handlers.Pairs
{
    public class RetrieveAllPairsQueryHandler : IRequestHandler<RetrieveAllPairsQuery, IEnumerable<Pair>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveAllPairsQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Pair>> Handle(RetrieveAllPairsQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAllPairsQuery();
            
            return await _mediator.Send(query, cancellationToken);
        }
    }
}