using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateRewindVaultsCommandHandler : IRequestHandler<CreateRewindVaultsCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindVaultsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindVaultsCommand request, CancellationToken cancellationToken)
        {
            // var governances = await _mediator.Send(new RetrieveMiningGovernancesByModifiedBlockQuery(request.RewindHeight));
            //
            // foreach (var governance in governances)
            // {
            //     await _mediator.Send(new MakeMiningGovernanceCommand(governance, request.RewindHeight, rewind: true));
            //     await _mediator.Send(new MakeGovernanceNominationsCommand(governance, request.RewindHeight));
            // }

            return true;
        }
    }
}
