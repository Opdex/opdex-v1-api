using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveMiningPoolByAddressQueryHandler : IRequestHandler<RetrieveMiningPoolByAddressQuery, MiningPool>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPoolByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<MiningPool> Handle(RetrieveMiningPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningPoolByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);
        }
    }
}