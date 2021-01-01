using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application.Handlers.Cirrus.Events
{
    public class RetrieveCirrusBurnEventsByPairQueryHandler : IRequestHandler<RetrieveCirrusBurnEventsByPairQuery, IEnumerable<object>>
    {
        private readonly ICirrusClient _cirrusClient;
        private readonly ILogger<RetrieveCirrusBurnEventsByPairQueryHandler> _logger;
        
        public RetrieveCirrusBurnEventsByPairQueryHandler(ICirrusClient cirrusClient, 
            ILogger<RetrieveCirrusBurnEventsByPairQueryHandler> logger)
        {
            _cirrusClient = cirrusClient ?? throw new ArgumentNullException(nameof(cirrusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<object>> Handle(RetrieveCirrusBurnEventsByPairQuery request, CancellationToken cancellationToken)
        {
            // Todo: Adjust for better response type
            // return await _cirrusClient.ReceiptSearchAsync(request.Contract, "BurnEvent", request.From, request.To, cancellationToken);
            return Enumerable.Empty<object>();
        }
    }
}