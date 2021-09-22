using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningPools
{
    public class RetrieveMiningPoolsByModifiedBlockQueryHandler : IRequestHandler<RetrieveMiningPoolsByModifiedBlockQuery, IEnumerable<MiningPool>>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPoolsByModifiedBlockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<MiningPool>> Handle(RetrieveMiningPoolsByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningPoolsByModifiedBlockQuery(request.BlockHeight), cancellationToken);
        }
    }
}
