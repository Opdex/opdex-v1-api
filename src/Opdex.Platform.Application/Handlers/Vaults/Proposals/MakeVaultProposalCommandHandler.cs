using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Proposals;

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
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(request.Proposal.VaultId), CancellationToken.None);

            var summary = await _mediator.Send(new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault.Address,
                                                                                                      request.Proposal.PublicId,
                                                                                                      request.BlockHeight), CancellationToken.None);

            request.Proposal.Update(summary, request.BlockHeight);
        }

        return await _mediator.Send(new PersistVaultProposalCommand(request.Proposal), CancellationToken.None);
    }
}
