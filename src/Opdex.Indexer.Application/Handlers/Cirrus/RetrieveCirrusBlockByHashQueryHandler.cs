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
    public class RetrieveCirrusBlockByHashQueryHandler : IRequestHandler<RetrieveCirrusBlockByHashQuery, BlockReceiptDto>
    {
        private readonly ILogger<RetrieveCirrusBlockByHashQueryHandler> _logger;
        private readonly IMediator _mediator;
        
        public RetrieveCirrusBlockByHashQueryHandler(ILogger<RetrieveCirrusBlockByHashQueryHandler> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<BlockReceiptDto> Handle(RetrieveCirrusBlockByHashQuery request, CancellationToken cancellationToken)
        {
            var query = new CallCirrusGetBlockByHashQuery(request.Hash);

            return _mediator.Send(query, cancellationToken);
        }
    }
}