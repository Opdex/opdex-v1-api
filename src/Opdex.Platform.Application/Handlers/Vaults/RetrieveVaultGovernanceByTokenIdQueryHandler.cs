using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults;

public class RetrieveVaultGovernanceByTokenIdQueryHandler : IRequestHandler<RetrieveVaultGovernanceByTokenIdQuery, VaultGovernance>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultGovernance> Handle(RetrieveVaultGovernanceByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultGovernanceByTokenIdQuery(request.TokenId, request.FindOrThrow), cancellationToken);
    }
}
