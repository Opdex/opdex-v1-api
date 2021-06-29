using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GeLiquidityPoolsWithFilterQueryHandler : IRequestHandler<GeLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPoolDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<LiquidityPool, LiquidityPoolDto> _assembler;

        public GeLiquidityPoolsWithFilterQueryHandler(IMediator mediator, IMapper mapper, IModelAssembler<LiquidityPool, LiquidityPoolDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public async Task<IEnumerable<LiquidityPoolDto>> Handle(GeLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress));

            var query = new RetrieveAllPoolsByMarketIdQuery(market.Id, request.Staking, request.Mining, request.Nominated, request.Skip, request.Take,
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
