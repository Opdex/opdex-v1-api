using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveVaultByIdQueryHandler : IRequestHandler<RetrieveVaultByIdQuery, Vault>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Vault> Handle(RetrieveVaultByIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectVaultByIdQuery(request.VaultId, request.FindOrThrow), cancellationToken);
        }
    }
}
