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
    public class RetrieveCirrusPairEventsQueryHandler : IRequestHandler<RetrieveCirrusPairEventsQuery, IEnumerable<object>>
    {
        private readonly ICirrusClient _cirrusClient;
        private readonly ILogger<RetrieveCirrusPairEventsQueryHandler> _logger;
        
        public RetrieveCirrusPairEventsQueryHandler(ICirrusClient cirrusClient, 
            ILogger<RetrieveCirrusPairEventsQueryHandler> logger)
        {
            _cirrusClient = cirrusClient ?? throw new ArgumentNullException(nameof(cirrusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<object>> Handle(RetrieveCirrusPairEventsQuery request, CancellationToken cancellationToken)
        {
            // Todo: Adjust for better response type
            return await _cirrusClient.ReceiptSearchAsync(request.Contract, "PairEvent", request.From, request.To, cancellationToken);
        }
    }
}