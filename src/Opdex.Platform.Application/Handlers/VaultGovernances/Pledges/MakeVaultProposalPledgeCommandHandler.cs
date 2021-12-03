using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Pledges;

public class MakeVaultProposalPledgeCommandHandler : IRequestHandler<MakeVaultProposalPledgeCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalPledgeCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<ulong> Handle(MakeVaultProposalPledgeCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistVaultProposalPledgeCommand(request.Pledge), CancellationToken.None);
    }
}
