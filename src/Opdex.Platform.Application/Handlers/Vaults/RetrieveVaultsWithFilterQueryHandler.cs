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
    public class RetrieveVaultsWithFilterQueryHandler : IRequestHandler<RetrieveVaultsWithFilterQuery, IEnumerable<Vault>>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<Vault>> Handle(RetrieveVaultsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new SelectVaultsWithFilterQuery(request.Cursor), cancellationToken);
        }
    }
}
