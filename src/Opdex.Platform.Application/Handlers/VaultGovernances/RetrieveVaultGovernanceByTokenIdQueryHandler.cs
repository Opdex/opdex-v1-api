using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances;

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
