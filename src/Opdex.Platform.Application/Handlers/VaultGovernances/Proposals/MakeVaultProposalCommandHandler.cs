using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;
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

    public async Task<ulong> Handle(MakeVaultProposalCommand request, CancellationToken cancellationToken)
    {
        if (request.RefreshProposal)
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(request.Proposal.VaultGovernanceId));

            var summary = await _mediator.Send(new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault.Address,
                                                                                                      request.Proposal.PublicId,
                                                                                                      request.BlockHeight));

            request.Proposal.Update(summary, request.BlockHeight);
        }

        return await _mediator.Send(new PersistVaultProposalCommand(request.Proposal), CancellationToken.None);
    }
}
