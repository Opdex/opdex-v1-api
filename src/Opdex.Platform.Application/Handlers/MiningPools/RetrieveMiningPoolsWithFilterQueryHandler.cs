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
    public class RetrieveMiningPoolsWithFilterQueryHandler : IRequestHandler<RetrieveMiningPoolsWithFilterQuery, IEnumerable<MiningPool>>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPoolsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<MiningPool>> Handle(RetrieveMiningPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new SelectMiningPoolsWithFilterQuery(request.Cursor), cancellationToken);
        }
    }
}
