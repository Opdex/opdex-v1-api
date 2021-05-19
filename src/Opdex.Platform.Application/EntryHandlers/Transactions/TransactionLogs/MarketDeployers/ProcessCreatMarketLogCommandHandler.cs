using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessCreateMarketLogCommandHandler : IRequestHandler<ProcessCreateMarketLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessCreateMarketLogCommandHandler> _logger;

        public ProcessCreateMarketLogCommandHandler(IMediator mediator, ILogger<ProcessCreateMarketLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateMarketLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(request.Log.Contract), CancellationToken.None);
                
                // Todo: Check to make sure the market doesn't exist first

                // Todo: Catch thrown errors or null check - standardization is coming...
                var stakingToken = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.StakingToken), CancellationToken.None);

                var market = new Market(request.Log.Market, deployer.Id, stakingToken.Id, request.Log.Owner, request.Log.AuthPoolCreators,
                    request.Log.AuthProviders, request.Log.AuthTraders, request.Log.Fee, request.BlockHeight, request.BlockHeight);
                
                var marketId = await _mediator.Send(new MakeMarketCommand(market), CancellationToken.None);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateMarketLog)}");
               
                return false;
            }
        }
    }
}