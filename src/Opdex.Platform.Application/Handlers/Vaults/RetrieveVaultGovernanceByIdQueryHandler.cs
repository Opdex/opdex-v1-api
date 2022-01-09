using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults;

public class RetrieveVaultGovernanceByIdQueryHandler : IRequestHandler<RetrieveVaultGovernanceByIdQuery, VaultGovernance>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceByIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultGovernance> Handle(RetrieveVaultGovernanceByIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultGovernanceByIdQuery(request.VaultId, request.FindOrThrow), cancellationToken);
    }
}
