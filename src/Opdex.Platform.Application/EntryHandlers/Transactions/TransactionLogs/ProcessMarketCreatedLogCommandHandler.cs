using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessMarketCreatedLogCommandHandler : IRequestHandler<ProcessMarketCreatedLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessMarketCreatedLogCommandHandler> _logger;

        public ProcessMarketCreatedLogCommandHandler(IMediator mediator, ILogger<ProcessMarketCreatedLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMarketCreatedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(request.Log.Contract), CancellationToken.None);
                
                // Todo: Check to make sure the market doesn't exist first
                var marketId = await _mediator.Send(new MakeMarketCommand(request.Log.Market, deployer.Id, null, request.Sender, request.Log.AuthPoolCreators,
                    request.Log.AuthProviders, request.Log.AuthTraders, request.Log.Fee), CancellationToken.None);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MarketCreatedLog)}");
               
                return false;
            }
        }
    }
}