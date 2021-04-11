using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveAllPoolsQueryHandler : IRequestHandler<RetrieveAllPoolsQuery, IEnumerable<LiquidityPool>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveAllPoolsQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPool>> Handle(RetrieveAllPoolsQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAllLiquidityPoolsQuery();
            
            return await _mediator.Send(query, cancellationToken);
        }
    }
}