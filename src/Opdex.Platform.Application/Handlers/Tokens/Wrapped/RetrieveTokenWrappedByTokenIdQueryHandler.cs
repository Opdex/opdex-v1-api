using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Wrapped;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Wrapped;

public class RetrieveTokenWrappedByTokenIdQueryHandler : IRequestHandler<RetrieveTokenWrappedByTokenIdQuery, TokenWrapped>
{
    private readonly IMediator _mediator;

    public RetrieveTokenWrappedByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TokenWrapped> Handle(RetrieveTokenWrappedByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectTokenWrappedByTokenIdQuery(request.TokenId, request.FindOrThrow), cancellationToken);
    }
}
