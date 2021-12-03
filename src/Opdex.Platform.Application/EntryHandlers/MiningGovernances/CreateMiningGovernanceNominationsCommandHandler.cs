using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningGovernances;

public class CreateMiningGovernanceNominationsCommandHandler : IRequestHandler<CreateMiningGovernanceNominationsCommand, bool>
{
    private readonly IMediator _mediator;

    public CreateMiningGovernanceNominationsCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(CreateMiningGovernanceNominationsCommand request, CancellationToken cancellationToken)
    {
        var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.MiningGovernance));

        return await _mediator.Send(new MakeMiningGovernanceNominationsCommand(miningGovernance, request.BlockHeight));
    }
}