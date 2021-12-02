using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances;

public class MakeVaultProposalVoteCommandHandler : IRequestHandler<MakeVaultProposalVoteCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalVoteCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultProposalVoteCommand request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new PersistVaultProposalVoteCommand(request.Vote), CancellationToken.None);
    }
}
