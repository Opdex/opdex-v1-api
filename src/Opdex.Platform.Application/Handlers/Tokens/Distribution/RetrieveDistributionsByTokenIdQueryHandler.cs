using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Distribution;

public class RetrieveDistributionsByTokenIdQueryHandler : IRequestHandler<RetrieveDistributionsByTokenIdQuery, IEnumerable<TokenDistribution>>
{
    private readonly IMediator _mediator;

    public RetrieveDistributionsByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<TokenDistribution>> Handle(RetrieveDistributionsByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectDistributionsByTokenIdQuery(request.TokenId), cancellationToken);
    }
}
