using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveTokensWithFilterQueryHandler : IRequestHandler<RetrieveTokensWithFilterQuery, IEnumerable<Token>>
    {
        private readonly IMediator _mediator;

        public RetrieveTokensWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<Token>> Handle(RetrieveTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectTokensWithFilterQuery(request.MarketId,
                                                                  request.LpToken,
                                                                  request.Skip,
                                                                  request.Take,
                                                                  request.SortBy,
                                                                  request.OrderBy,
                                                                  request.Tokens), cancellationToken);
        }
    }
}
