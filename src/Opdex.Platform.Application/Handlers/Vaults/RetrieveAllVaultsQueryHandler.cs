using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveAllVaultsQueryHandler : IRequestHandler<RetrieveAllVaultsQuery, IEnumerable<Vault>>
    {
        private readonly IMediator _mediator;

        public RetrieveAllVaultsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<Vault>> Handle(RetrieveAllVaultsQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectAllVaultsQuery(), cancellationToken);
        }
    }
}
