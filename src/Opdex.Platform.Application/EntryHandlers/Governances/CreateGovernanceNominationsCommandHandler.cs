using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class CreateGovernanceNominationsCommandHandler : IRequestHandler<CreateGovernanceNominationsCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateGovernanceNominationsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateGovernanceNominationsCommand request, CancellationToken cancellationToken)
        {
            var governance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Governance));

            return await _mediator.Send(new MakeGovernanceNominationsCommand(governance, request.BlockHeight));
        }
    }
}
