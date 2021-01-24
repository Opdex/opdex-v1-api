using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opdex.Core.Common;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;

namespace Opdex.Indexer.Application.Handlers.Cirrus.Events
{
    public class RetrieveCirrusPairEventsQueryHandler : IRequestHandler<RetrieveCirrusPairEventsQuery, IEnumerable<PairCreatedEvent>>
    {
        private readonly OpdexConfiguration _opdexConfiguration;
        private readonly IMediator _mediator;
        private readonly ILogger<RetrieveCirrusPairEventsQueryHandler> _logger;

        public RetrieveCirrusPairEventsQueryHandler(IOptions<OpdexConfiguration> opdexConfiguration, IMediator mediator,
            ILogger<RetrieveCirrusPairEventsQueryHandler> logger)
        {
            _opdexConfiguration = opdexConfiguration.Value ?? throw new ArgumentNullException(nameof(opdexConfiguration));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<PairCreatedEvent>> Handle(RetrieveCirrusPairEventsQuery request, CancellationToken cancellationToken)
        {
            var query = new CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery(
                _opdexConfiguration.ControllerContract, nameof(PairCreatedEvent), request.FromBlock, request.ToBlock);
            
            var transactionReceipts = await _mediator.Send(query, cancellationToken);

            var pairEvents = transactionReceipts
                .SelectMany(t => t.Events
                    .Where(e => e.Event.EventType == nameof(PairCreatedEvent))
                    .Select(e => e.Event as PairCreatedEvent));

            return pairEvents;
        }
    }
}