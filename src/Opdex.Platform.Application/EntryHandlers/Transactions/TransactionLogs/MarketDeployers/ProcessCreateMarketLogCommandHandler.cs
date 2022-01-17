using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers;

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
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(request.BlockHeight), CancellationToken.None);

            // Get deployer
            var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (deployer == null) return false;

            // Check if market exists, skip if so
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Market, findOrThrow: false));
            if (market != null) return true;

            // Check if its a staking market, get staking token details
            Token stakingToken = null;
            if (!request.Log.StakingToken.IsZero)
            {
                var stakingAttributes = new[] { TokenAttributeType.Staking };
                var tokenId = await _mediator.Send(new CreateTokenCommand(request.Log.StakingToken, stakingAttributes, request.BlockHeight));
                if (tokenId == 0)
                {
                    _logger.LogError("Unable to create market staking token");
                    return false;
                }

                stakingToken = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.StakingToken, findOrThrow: false), CancellationToken.None);
            }

            // Create market
            market = new Market(request.Log.Market, deployer.Id, stakingToken?.Id ?? 0, request.Log.Owner, request.Log.AuthPoolCreators,
                                request.Log.AuthProviders, request.Log.AuthTraders, request.Log.TransactionFee,
                                request.Log.EnableMarketFee, request.BlockHeight);

            var marketId = await _mediator.Send(new MakeMarketCommand(market, request.BlockHeight));
            if (marketId == 0) return false;

            var snapshot = new MarketSnapshot(marketId, SnapshotType.Daily, block.MedianTime);
            var marketSnapshot = await _mediator.Send(new MakeMarketSnapshotCommand(snapshot, request.BlockHeight), CancellationToken.None);
            if (!marketSnapshot) _logger.LogWarning("Unexpected error creating new market snapshot.");

            // Create Router
            var router = await _mediator.Send(new RetrieveMarketRouterByAddressQuery(request.Log.Router, findOrThrow: false));
            if (router != null) return true;

            router = new MarketRouter(request.Log.Router, marketId, true, request.BlockHeight);
            var routerPersisted = await _mediator.Send(new MakeMarketRouterCommand(router), CancellationToken.None);

            // Create staking token vault and governance contracts
            if (stakingToken != null)
            {
                // Get token summary
                var stakingTokenSummary = await _mediator.Send(new RetrieveStakingTokenContractSummaryQuery(stakingToken.Address, request.BlockHeight,
                                                                                                            includeVault: true, includeMiningGovernance: true));

                if (stakingTokenSummary.Vault == null || stakingTokenSummary.MiningGovernance == null)
                {
                    _logger.LogError("Missing Vault and Mining Governance from staking token summary.");
                    return false;
                }

                var vaultId = await _mediator.Send(new CreateVaultCommand(stakingTokenSummary.Vault.Value, stakingToken.Id, request.BlockHeight));

                var miningGovernanceId = await _mediator.Send(new CreateMiningGovernanceCommand(stakingTokenSummary.MiningGovernance.Value,
                                                                                                stakingToken.Id, request.BlockHeight));

                if (vaultId == 0)
                {
                    _logger.LogError("Unexpected vaultId response from creation");
                    return false;
                }

                if (miningGovernanceId == 0)
                {
                    _logger.LogError("Unexpected miningGovernanceId response from creation");
                    return false;
                }
            }

            return routerPersisted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CreateMarketLog)}");

            return false;
        }
    }
}
