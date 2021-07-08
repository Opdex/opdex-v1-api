using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
    public class RetrieveMiningGovernanceByAddressQueryHandler  : IRequestHandler<RetrieveMiningGovernanceByAddressQuery, MiningGovernance>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningGovernanceByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<MiningGovernance> Handle(RetrieveMiningGovernanceByAddressQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningGovernanceByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);
        }
    }
}
