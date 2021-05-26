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
    public class ProcessCreateMarketLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCreateMarketLogCommand, bool>
    {
        private readonly ILogger<ProcessCreateMarketLogCommandHandler> _logger;

        public ProcessCreateMarketLogCommandHandler(IMediator mediator, ILogger<ProcessCreateMarketLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateMarketLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                // Get deployer
                var deployerQuery = new RetrieveDeployerByAddressQuery(request.Log.Contract, findOrThrow: true);
                var deployer = await _mediator.Send(deployerQuery, CancellationToken.None);

                // Check if market exists, skip if so
                var marketQuery = new RetrieveMarketByAddressQuery(request.Log.Market, findOrThrow: false);
                var market = await _mediator.Send(marketQuery, CancellationToken.None);

                // Skip if market already exists
                if (market != null)
                {
                    return true;
                }

                // Get potential market staking token
                var stakingTokenQuery = new RetrieveTokenByAddressQuery(request.Log.StakingToken, findOrThrow: false);
                var stakingToken = await _mediator.Send(stakingTokenQuery, CancellationToken.None);
                var stakingTokenId = stakingToken?.Id ?? 0;
                
                // Create market
                market = new Market(request.Log.Market, deployer.Id, stakingTokenId, request.Log.Owner, request.Log.AuthPoolCreators,
                    request.Log.AuthProviders, request.Log.AuthTraders, request.Log.Fee, request.BlockHeight);
                
                var marketId = await _mediator.Send(new MakeMarketCommand(market), CancellationToken.None);

                return marketId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateMarketLog)}");
               
                return false;
            }
        }
    }
}