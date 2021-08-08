using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveMiningPositionsWithFilterQueryHandler : IRequestHandler<RetrieveMiningPositionsWithFilterQuery, IEnumerable<AddressMining>>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPositionsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<AddressMining>> Handle(RetrieveMiningPositionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningPositionsWithFilterQuery(request.Address, request.Cursor), cancellationToken);
        }
    }
}
