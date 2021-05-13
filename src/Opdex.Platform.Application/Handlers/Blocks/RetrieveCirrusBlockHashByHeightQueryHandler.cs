using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Application.Handlers.Blocks
{
    public class RetrieveCirrusBlockHashByHeightQueryHandler : IRequestHandler<RetrieveCirrusBlockHashByHeightQuery, string>
    {
        private readonly IMediator _mediator;

        public RetrieveCirrusBlockHashByHeightQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(RetrieveCirrusBlockHashByHeightQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetBlockHashByHeightQuery(request.Height), cancellationToken);
        }
    }
}