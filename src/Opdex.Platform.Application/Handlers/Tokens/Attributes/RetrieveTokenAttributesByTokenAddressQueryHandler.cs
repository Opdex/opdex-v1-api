using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Attributes;

public class RetrieveTokenAttributesByTokenAddressQueryHandler : IRequestHandler<RetrieveTokenAttributesByTokenAddressQuery, IEnumerable<TokenAttribute>>
{
    private readonly IMediator _mediator;

    public RetrieveTokenAttributesByTokenAddressQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<TokenAttribute>> Handle(RetrieveTokenAttributesByTokenAddressQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectTokenAttributesByTokenAddressQuery(request.Token), cancellationToken);
    }
}
