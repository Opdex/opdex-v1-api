using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;

namespace Opdex.Indexer.Application.Handlers.Cirrus
{
    public class RetrieveCirrusCurrentBlockQueryHandler: IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockDto>
    {
        private readonly ICirrusClient _cirrusClient;
        private readonly ILogger<RetrieveCirrusCurrentBlockQueryHandler> _logger;
        
        public RetrieveCirrusCurrentBlockQueryHandler(ICirrusClient cirrusClient, 
            ILogger<RetrieveCirrusCurrentBlockQueryHandler> logger)
        {
            _cirrusClient = cirrusClient ?? throw new ArgumentNullException(nameof(cirrusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Task<BlockDto> Handle(RetrieveCirrusCurrentBlockQuery request, CancellationToken cancellationToken)
        {
            // Todo: Use node status endpoint to check to the current block
            throw new System.NotImplementedException();
        }
    }
}