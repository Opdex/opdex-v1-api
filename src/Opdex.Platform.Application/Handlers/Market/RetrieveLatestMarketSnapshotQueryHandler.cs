using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.Queries.Market;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Market;

namespace Opdex.Platform.Application.Handlers.Market
{
    public class RetrieveLatestMarketSnapshotQueryHandler: IRequestHandler<RetrieveLatestMarketSnapshotQuery, MarketSnapshot>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveLatestMarketSnapshotQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<MarketSnapshot> Handle(RetrieveLatestMarketSnapshotQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLatestMarketSnapshotQuery(), cancellationToken);
        }
    }
}