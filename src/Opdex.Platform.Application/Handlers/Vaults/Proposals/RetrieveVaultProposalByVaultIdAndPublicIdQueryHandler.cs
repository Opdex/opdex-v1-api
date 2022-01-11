using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Proposals;

public class RetrieveVaultProposalByVaultIdAndPublicIdQueryHandler : IRequestHandler<RetrieveVaultProposalByVaultIdAndPublicIdQuery, VaultProposal>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalByVaultIdAndPublicIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultProposal> Handle(RetrieveVaultProposalByVaultIdAndPublicIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalByVaultIdAndPublicIdQuery(request.VaultId, request.PublicId, request.FindOrThrow), cancellationToken);
    }
}
