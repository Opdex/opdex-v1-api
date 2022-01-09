using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults;

public class RetrieveVaultGovernanceByAddressQueryHandler : IRequestHandler<RetrieveVaultGovernanceByAddressQuery, VaultGovernance>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceByAddressQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultGovernance> Handle(RetrieveVaultGovernanceByAddressQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultGovernanceByAddressQuery(request.Vault, request.FindOrThrow), cancellationToken);
    }
}
