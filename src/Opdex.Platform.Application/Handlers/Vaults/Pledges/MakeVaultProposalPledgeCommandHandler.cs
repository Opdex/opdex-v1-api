using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Pledges;

public class MakeVaultProposalPledgeCommandHandler : IRequestHandler<MakeVaultProposalPledgeCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalPledgeCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultProposalPledgeCommand request, CancellationToken cancellationToken)
    {
        if (request.RefreshBalance)
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(request.Pledge.VaultId), CancellationToken.None);
            var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(request.Pledge.ProposalId), CancellationToken.None);
            var pledge = await _mediator.Send(new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault.Address,
                                                                                                              proposal.PublicId,
                                                                                                              request.Pledge.Pledger,
                                                                                                              request.BlockHeight), CancellationToken.None);

            request.Pledge.UpdateBalance(pledge, request.BlockHeight);
        }

        return await _mediator.Send(new PersistVaultProposalPledgeCommand(request.Pledge), CancellationToken.None);
    }
}
