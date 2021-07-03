using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveVaultQueryHandler : IRequestHandler<RetrieveVaultQuery, Vault>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<Vault> Handle(RetrieveVaultQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectVaultQuery(request.FindOrThrow), cancellationToken);
        }
    }
}
