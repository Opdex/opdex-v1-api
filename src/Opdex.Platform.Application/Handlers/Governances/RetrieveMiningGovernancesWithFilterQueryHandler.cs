using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
    public class RetrieveMiningGovernancesWithFilterQueryHandler : IRequestHandler<RetrieveMiningGovernancesWithFilterQuery, IEnumerable<MiningGovernance>>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningGovernancesWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<MiningGovernance>> Handle(RetrieveMiningGovernancesWithFilterQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new SelectMiningGovernancesWithFilterQuery(request.Cursor), cancellationToken);
        }
    }
}
