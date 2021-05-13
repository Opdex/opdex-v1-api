using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault;

namespace Opdex.Platform.Application.Handlers.vault
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
            return _mediator.Send(new SelectVaultQuery(), cancellationToken);
        }
    }
}