using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveVaultByTokenIdQueryHandler : IRequestHandler<RetrieveVaultByTokenIdQuery, Vault>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultByTokenIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Vault> Handle(RetrieveVaultByTokenIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectVaultByTokenIdQuery(request.TokenId, request.FindOrThrow), cancellationToken);
        }
    }
}
