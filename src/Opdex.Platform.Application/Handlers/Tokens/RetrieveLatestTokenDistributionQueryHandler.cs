using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveLatestTokenDistributionQueryHandler : IRequestHandler<RetrieveLatestTokenDistributionQuery, TokenDistribution>
    {
        private readonly IMediator _mediator;

        public RetrieveLatestTokenDistributionQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<TokenDistribution> Handle(RetrieveLatestTokenDistributionQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLatestTokenDistributionQuery(), cancellationToken);
        }
    }
}