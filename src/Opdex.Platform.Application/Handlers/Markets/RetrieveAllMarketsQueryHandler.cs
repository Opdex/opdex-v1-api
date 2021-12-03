using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Application.Handlers.Markets;

public class RetrieveAllMarketsQueryHandler : IRequestHandler<RetrieveAllMarketsQuery, IEnumerable<Market>>
{
    private readonly IMediator _mediator;

    public RetrieveAllMarketsQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<Market>> Handle(RetrieveAllMarketsQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectAllMarketsQuery(), cancellationToken);
    }
}