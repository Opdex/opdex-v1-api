using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Handlers.Blocks
{
    public class RetrieveCirrusCurrentBlockQueryHandler
        : IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockReceipt>
    {
        private readonly ILogger<RetrieveCirrusCurrentBlockQueryHandler> _logger;
        private readonly IMediator _mediator;

        public RetrieveCirrusCurrentBlockQueryHandler(ILogger<RetrieveCirrusCurrentBlockQueryHandler> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<BlockReceipt> Handle(RetrieveCirrusCurrentBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetCurrentBlockQuery(), cancellationToken);
        }
    }
}