using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Proposals;

public class RetrieveVaultProposalByIdQueryHandler : IRequestHandler<RetrieveVaultProposalByIdQuery, VaultProposal>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalByIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultProposal> Handle(RetrieveVaultProposalByIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalByIdQuery(request.ProposalId, request.FindOrThrow), cancellationToken);
    }
}
