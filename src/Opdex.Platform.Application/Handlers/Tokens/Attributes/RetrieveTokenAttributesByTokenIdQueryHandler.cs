using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Attributes;

public class RetrieveTokenAttributesByTokenIdQueryHandler : IRequestHandler<RetrieveTokenAttributesByTokenIdQuery, IEnumerable<TokenAttribute>>
{
    private readonly IMediator _mediator;

    public RetrieveTokenAttributesByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<TokenAttribute>> Handle(RetrieveTokenAttributesByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectTokenAttributesByTokenIdQuery(request.TokenId), cancellationToken);
    }
}
