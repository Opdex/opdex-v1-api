using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
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
                var deployer = await _mediator.Send( new RetrieveDeployerByAddressQuery(request.Log.Contract, findOrThrow: true));

                // Check if market exists, skip if so
                var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Market, findOrThrow: false));

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
                                    request.Log.AuthProviders, request.Log.AuthTraders, request.Log.TransactionFee,
                                    request.Log.EnableMarketFee, request.BlockHeight);

                var marketId = await _mediator.Send(new MakeMarketCommand(market), CancellationToken.None);

                // Create Router
                var router = await _mediator.Send(new RetrieveMarketRouterByAddressQuery(request.Log.Router, findOrThrow: false));

                if (router == null)
                {
                    router = new MarketRouter(request.Log.Router, marketId, true, request.BlockHeight);
                    await _mediator.Send(new MakeMarketRouterCommand(router), CancellationToken.None);
                }

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
