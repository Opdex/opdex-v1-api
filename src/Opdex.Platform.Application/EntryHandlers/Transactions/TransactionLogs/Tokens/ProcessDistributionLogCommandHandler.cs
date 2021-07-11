using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System.Linq;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessDistributionLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessDistributionLogCommand, bool>
    {
        private readonly ILogger<ProcessDistributionLogCommandHandler> _logger;

        public ProcessDistributionLogCommandHandler(IMediator mediator, ILogger<ProcessDistributionLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessDistributionLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var vaultQuery = new RetrieveVaultQuery(findOrThrow: true);
                var vault = await _mediator.Send(vaultQuery, CancellationToken.None);

                var tokenQuery = new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: true);
                var token = await _mediator.Send(tokenQuery, CancellationToken.None);

                if (vault.TokenId != token.Id)
                {
                    return true;
                }

                var blockHeight = request.BlockHeight;
                var vaultAmount = request.Log.VaultAmount;
                var miningAmount = request.Log.MiningAmount;
                var periodIndex = request.Log.PeriodIndex;
                var nextPeriodIndex = periodIndex + 1;

                // First period, index mining governance nominations
                if (periodIndex == 0)
                {
                    var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(token.Id));
                    await InitializeNominations(miningGovernance.Address);
                }

                // Get the period duration (per year) from the smart contract
                var periodDurationRequest = new RetrieveCirrusLocalCallSmartContractQuery(token.Address, "get_PeriodDuration");
                var periodDurationSerialized = await _mediator.Send(periodDurationRequest, CancellationToken.None);
                var periodDuration = periodDurationSerialized.DeserializeValue<ulong>();

                var nextDistributionBlock = vault.Genesis + (periodDuration * nextPeriodIndex);

                var latestDistributionQuery = new RetrieveLatestTokenDistributionQuery(findOrThrow: false);
                var latestDistribution = await _mediator.Send(latestDistributionQuery, CancellationToken.None);

                if (latestDistribution != null && latestDistribution.PeriodIndex >= periodIndex)
                {
                    return true;
                }

                var distribution = new TokenDistribution(vaultAmount, miningAmount, (int)periodIndex, blockHeight, nextDistributionBlock, blockHeight);
                return await _mediator.Send(new MakeTokenDistributionCommand(distribution), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(DistributionLog)}");

                return false;
            }
        }

        private async Task InitializeNominations(string miningGovernance)
        {
            var block = await _mediator.Send(new RetrieveLatestBlockQuery());

            var nominatedPools = await _mediator.Send(new RetrieveCirrusMiningGovernanceNominationsQuery(miningGovernance));

            var nominatedLiquidityPools = await Task.WhenAll(
                nominatedPools.Select(nomination => _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(nomination.StakingPool))));

            var nominatedMiningPools = await Task.WhenAll(
                nominatedLiquidityPools.Select(pool => _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id))));

            var nominations = nominatedMiningPools.Select(miningPool => new MiningGovernanceNomination(miningPool.LiquidityPoolId,
                                                                                                       miningPool.Id,
                                                                                                       true,
                                                                                                       "1",
                                                                                                       block.Height));

            await Task.WhenAll(
                nominations.Select(nomination => _mediator.Send(new MakeMiningGovernanceNominationCommand(nomination))));
        }
    }
}
