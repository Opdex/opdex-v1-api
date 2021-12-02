using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances;

public class MakeVaultProposalCommandHandler : IRequestHandler<MakeVaultProposalCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultProposalCommand request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new PersistVaultProposalCommand(request.Proposal), CancellationToken.None);
    }
}
