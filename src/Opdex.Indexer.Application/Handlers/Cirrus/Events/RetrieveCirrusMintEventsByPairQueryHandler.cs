using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application.Handlers.Cirrus.Events
{
    public class RetrieveCirrusMintEventsByPairQueryHandler : IRequestHandler<RetrieveCirrusMintEventsByPairQuery, IEnumerable<object>>
    {
        private readonly ICirrusClient _cirrusClient;
        private readonly ILogger<RetrieveCirrusMintEventsByPairQueryHandler> _logger;
        
        public RetrieveCirrusMintEventsByPairQueryHandler(ICirrusClient cirrusClient, 
            ILogger<RetrieveCirrusMintEventsByPairQueryHandler> logger)
        {
            _cirrusClient = cirrusClient ?? throw new ArgumentNullException(nameof(cirrusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<object>> Handle(RetrieveCirrusMintEventsByPairQuery request, CancellationToken cancellationToken)
        {
            // Todo: Adjust for better response type
            return await _cirrusClient.ReceiptSearchAsync(request.Contract, "MintEvent", request.From, request.To, cancellationToken);
        }
    }
}