using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Distribution;

public class RetrieveLatestTokenDistributionByTokenIdQueryHandler : IRequestHandler<RetrieveLatestTokenDistributionByTokenIdQuery, TokenDistribution>
{
    private readonly IMediator _mediator;

    public RetrieveLatestTokenDistributionByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<TokenDistribution> Handle(RetrieveLatestTokenDistributionByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectLatestTokenDistributionByTokenIdQuery(request.TokenId, request.FindOrThrow), cancellationToken);
    }
}
