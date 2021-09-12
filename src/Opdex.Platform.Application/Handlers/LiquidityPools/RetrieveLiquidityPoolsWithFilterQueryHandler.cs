using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolsWithFilterQueryHandler : IRequestHandler<RetrieveLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<LiquidityPool>> Handle(RetrieveLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectLiquidityPoolsWithFilterQuery(request.MarketId, request.Staking, request.Mining, request.Nominated,
                                                                request.Skip, request.Take, request.SortBy, request.OrderBy, request.Pools);

            return await _mediator.Send(query, cancellationToken);
        }
    }
}
