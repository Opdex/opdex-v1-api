using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances.Nominations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances.Nominations
{
    public class RetrieveActiveGovernanceNominationsByGovernanceIdQueryHandler
        : IRequestHandler<RetrieveActiveGovernanceNominationsByGovernanceIdQuery, IEnumerable<MiningGovernanceNomination>>
    {
        private readonly IMediator _mediator;

        public RetrieveActiveGovernanceNominationsByGovernanceIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<MiningGovernanceNomination>> Handle(RetrieveActiveGovernanceNominationsByGovernanceIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectActiveGovernanceNominationsByGovernanceIdQuery(request.GovernanceId), cancellationToken);
        }
    }
}
