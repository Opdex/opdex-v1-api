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
    public class RetrieveCirrusSwapEventsByPairQueryHandler : IRequestHandler<RetrieveCirrusSwapEventsByPairQuery, IEnumerable<object>>
    {
        private readonly ILogger<RetrieveCirrusSwapEventsByPairQueryHandler> _logger;
        
        public RetrieveCirrusSwapEventsByPairQueryHandler(ILogger<RetrieveCirrusSwapEventsByPairQueryHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<object>> Handle(RetrieveCirrusSwapEventsByPairQuery request, CancellationToken cancellationToken)
        {
            // Todo: Adjust for better response type
            // return await _cirrusClient.ReceiptSearchAsync(request.Contract, "SwapEvent", request.From, request.To, cancellationToken);
            return Enumerable.Empty<object>();
        }
    }
}