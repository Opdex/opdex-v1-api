using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessChangeMarketOwnerLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessChangeMarketOwnerLogCommand, bool>
    {
        private readonly ILogger<ProcessChangeMarketOwnerLogCommandHandler> _logger;

        public ProcessChangeMarketOwnerLogCommandHandler(IMediator mediator, ILogger<ProcessChangeMarketOwnerLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessChangeMarketOwnerLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var marketQuery = new RetrieveMarketByAddressQuery(request.Log.Contract, findOrThrow: true);
                var market = await _mediator.Send(marketQuery, CancellationToken.None);
                
                market.SetOwner(request.Log, request.BlockHeight);

                var marketCommand = new MakeMarketCommand(market);
                var marketId = await _mediator.Send(marketCommand, CancellationToken.None);
                
                return marketId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ChangeMarketOwnerLog)}");
               
                return false;
            }
        }
    }
}