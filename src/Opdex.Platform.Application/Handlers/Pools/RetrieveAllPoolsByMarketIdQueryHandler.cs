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
    public class RetrieveAllPoolsByMarketIdQueryHandler : IRequestHandler<RetrieveAllPoolsByMarketIdQuery, IEnumerable<LiquidityPool>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RetrieveAllPoolsByMarketIdQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPool>> Handle(RetrieveAllPoolsByMarketIdQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAllLiquidityPoolsByMarketIdQuery(request.MarketId);

            return await _mediator.Send(query, cancellationToken);
        }
    }
}