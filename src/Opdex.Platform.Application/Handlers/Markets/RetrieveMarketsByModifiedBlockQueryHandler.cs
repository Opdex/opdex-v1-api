using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets;

public class RetrieveMarketsByModifiedBlockQueryHandler  : IRequestHandler<RetrieveMarketsByModifiedBlockQuery, IEnumerable<Market>>
{
    private readonly IMediator _mediator;

    public RetrieveMarketsByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<Market>> Handle(RetrieveMarketsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectMarketsByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}