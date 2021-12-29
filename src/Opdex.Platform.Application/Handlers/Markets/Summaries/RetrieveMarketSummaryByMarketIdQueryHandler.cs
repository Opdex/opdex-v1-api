using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Summaries;

public class RetrieveMarketSummaryByMarketIdQueryHandler
    : IRequestHandler<RetrieveMarketSummaryByMarketIdQuery, MarketSummary>
{
    private readonly IMediator _mediator;

    public RetrieveMarketSummaryByMarketIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<MarketSummary> Handle(RetrieveMarketSummaryByMarketIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectMarketSummaryByMarketIdQuery(request.MarketId, request.FindOrThrow), cancellationToken);
    }
}
