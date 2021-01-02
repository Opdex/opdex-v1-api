using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application.Handlers.Cirrus.Events
{
    public class RetrieveCirrusBurnEventsByPairQueryHandler : IRequestHandler<RetrieveCirrusBurnEventsByPairQuery, IEnumerable<object>>
    {
        private readonly ILogger<RetrieveCirrusBurnEventsByPairQueryHandler> _logger;
        
        public RetrieveCirrusBurnEventsByPairQueryHandler(ILogger<RetrieveCirrusBurnEventsByPairQueryHandler> logger)
        {
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