using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Wrapped;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Wrapped;

public class RetrieveTokenChainByTokenIdQueryHandler : IRequestHandler<RetrieveTokenChainByTokenIdQuery, TokenChain>
{
    private readonly IMediator _mediator;

    public RetrieveTokenChainByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TokenChain> Handle(RetrieveTokenChainByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectTokenChainByTokenIdQuery(request.TokenId, request.FindOrThrow), cancellationToken);
    }
}
