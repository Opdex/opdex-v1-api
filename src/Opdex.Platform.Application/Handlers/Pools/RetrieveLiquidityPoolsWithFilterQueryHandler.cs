using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolsWithFilterQueryHandler : IRequestHandler<RetrieveLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RetrieveLiquidityPoolsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPool>> Handle(RetrieveLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectLiquidityPoolsWithFilterQuery(request.MarketId, request.Staking, request.Mining, request.Nominated,
                                                                request.Skip, request.Take, request.SortBy, request.OrderBy, request.Pools);

            return await _mediator.Send(query, cancellationToken);
        }
    }
}
