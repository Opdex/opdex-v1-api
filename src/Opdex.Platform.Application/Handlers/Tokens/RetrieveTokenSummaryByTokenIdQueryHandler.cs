using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens;

public class RetrieveTokenSummaryByTokenIdQueryHandler : IRequestHandler<RetrieveTokenSummaryByTokenIdQuery, TokenSummary>
{
    private readonly IMediator _mediator;

    public RetrieveTokenSummaryByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TokenSummary> Handle(RetrieveTokenSummaryByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectTokenSummaryByTokenIdQuery(request.TokenId), cancellationToken);
    }
}
