using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveVaultByAddressQueryHandler : IRequestHandler<RetrieveVaultByAddressQuery, Vault>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Vault> Handle(RetrieveVaultByAddressQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new SelectVaultByAddressQuery(request.Vault, request.FindOrThrow), cancellationToken);
        }
    }
}
