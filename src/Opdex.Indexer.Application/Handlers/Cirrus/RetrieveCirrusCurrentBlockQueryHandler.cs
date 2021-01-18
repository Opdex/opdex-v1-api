using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;

namespace Opdex.Indexer.Application.Handlers.Cirrus
{
    public class RetrieveCirrusCurrentBlockQueryHandler
        : IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockReceiptDto>
    {
        private readonly ILogger<RetrieveCirrusCurrentBlockQueryHandler> _logger;
        private readonly IMediator _mediator;
        
        public RetrieveCirrusCurrentBlockQueryHandler(ILogger<RetrieveCirrusCurrentBlockQueryHandler> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<BlockReceiptDto> Handle(RetrieveCirrusCurrentBlockQuery request, CancellationToken cancellationToken)
        {
            var query = new CallCirrusGetCurrentBlockQuery();

            return await _mediator.Send(query, cancellationToken);
        }
    }
}