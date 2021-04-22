using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Market;
using Opdex.Platform.Application.Abstractions.Queries.Market;

namespace Opdex.Platform.Application.EntryHandlers.Market
{
    public class GetLatestMarketSnapshotQueryHandler : IRequestHandler<GetLatestMarketSnapshotQuery, MarketSnapshotDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public GetLatestMarketSnapshotQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MarketSnapshotDto> Handle(GetLatestMarketSnapshotQuery request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RetrieveLatestMarketSnapshotQuery(), cancellationToken);

            return _mapper.Map<MarketSnapshotDto>(result);
        }
    }
}