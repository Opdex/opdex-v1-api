using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Proposals;

public class MakeVaultProposalCommandHandler : IRequestHandler<MakeVaultProposalCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<ulong> Handle(MakeVaultProposalCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistVaultProposalCommand(request.Proposal), CancellationToken.None);
    }
}
