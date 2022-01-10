using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets;

public class RetrieveMarketsWithFilterQueryHandler : IRequestHandler<RetrieveMarketsWithFilterQuery, IEnumerable<Market>>
{
    private readonly IMediator _mediator;

    public RetrieveMarketsWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<Market>> Handle(RetrieveMarketsWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectMarketsWithFilterQuery(request.Cursor), cancellationToken);
    }
}
