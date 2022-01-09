using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Pledges;

public class RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler
    : IRequestHandler<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery, VaultProposalPledge>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultProposalPledge> Handle(RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(request.VaultId, request.ProposalId, request.Pledger,
                                                                                                 request.FindOrThrow), cancellationToken);
    }
}
