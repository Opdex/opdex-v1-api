using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Mining;

public class RetrieveMiningPositionsByModifiedBlockQueryHandler : IRequestHandler<RetrieveMiningPositionsByModifiedBlockQuery, IEnumerable<AddressMining>>
{
    private readonly IMediator _mediator;

    public RetrieveMiningPositionsByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    public async Task<IEnumerable<AddressMining>> Handle(RetrieveMiningPositionsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectMiningPositionsByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}