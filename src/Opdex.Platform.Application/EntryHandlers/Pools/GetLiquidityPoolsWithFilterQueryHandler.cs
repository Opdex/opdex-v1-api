using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetLiquidityPoolsWithFilterQueryHandler : IRequestHandler<GetLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPoolDto>>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<LiquidityPool, LiquidityPoolDto> _assembler;

        public GetLiquidityPoolsWithFilterQueryHandler(IMediator mediator, IModelAssembler<LiquidityPool, LiquidityPoolDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public async Task<IEnumerable<LiquidityPoolDto>> Handle(GetLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress));

            var query = new RetrieveLiquidityPoolsWithFilterQuery(market.Id, request.Staking, request.Mining, request.Nominated, request.Skip, request.Take,
                                                                  request.SortBy, request.OrderBy, request.Pools);

            var pools = await _mediator.Send(query, cancellationToken);

            var poolDtos = new List<LiquidityPoolDto>();

            foreach (var pool in pools)
            {
                var poolDto = await _assembler.Assemble(pool);
                poolDtos.Add(poolDto);
            }

            return poolDtos;
        }
    }
}
