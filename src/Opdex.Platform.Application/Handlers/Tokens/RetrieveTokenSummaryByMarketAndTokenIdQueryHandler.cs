using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens;

public class RetrieveTokenSummaryByMarketAndTokenIdQueryHandler : IRequestHandler<RetrieveTokenSummaryByMarketAndTokenIdQuery, TokenSummary>
{
    private readonly IMediator _mediator;

    public RetrieveTokenSummaryByMarketAndTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<TokenSummary> Handle(RetrieveTokenSummaryByMarketAndTokenIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectTokenSummaryByMarketAndTokenIdQuery(request.MarketId, request.TokenId, request.FindOrThrow), cancellationToken);
    }
}