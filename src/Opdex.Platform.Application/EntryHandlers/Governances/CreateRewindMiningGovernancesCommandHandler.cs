using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class CreateRewindMiningGovernancesCommandHandler : IRequestHandler<CreateRewindMiningGovernancesCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindMiningGovernancesCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindMiningGovernancesCommand request, CancellationToken cancellationToken)
        {
            var governances = await _mediator.Send(new RetrieveMiningGovernancesByModifiedBlockQuery(request.RewindHeight));

            foreach (var governance in governances)
            {
                await _mediator.Send(new MakeMiningGovernanceCommand(governance, request.RewindHeight, rewind: true));
                await _mediator.Send(new MakeGovernanceNominationsCommand(governance, request.RewindHeight));
            }

            return true;
        }
    }
}
