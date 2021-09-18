using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System.Linq;
using Opdex.Platform.Common.Models;

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

                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: true));
                var vault = await _mediator.Send(new RetrieveVaultByTokenIdQuery(token.Id, findOrThrow: true));
                var governance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(token.Id, findOrThrow: true));

                // process vault balances
                var vaultResult = await _mediator.Send(new CreateAddressBalanceCommand(vault.Address, token.Address, request.BlockHeight));
                if (vaultResult <= 0) return false;

                // process governance balances
                var governanceResult = await _mediator.Send(new CreateAddressBalanceCommand(governance.Address, token.Address, request.BlockHeight));
                if (governanceResult <= 0) return false;

                // Process vault unassigned supply updates. (The vault's TotalSupply property in contract represents the amount of tokens
                // available to the vault to distribute. The vault's balance could be 100 tokens but the TotalSupply could have only 50 tokens
                // left that have not been distributed).
                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var unassignedSupply = await _mediator.Send(new RetrieveCirrusVaultTotalSupplyQuery(vault.Address, request.BlockHeight));

                    vault.SetUnassignedSupply(unassignedSupply, request.BlockHeight);

                    if (request.Log.PeriodIndex == 0)
                    {
                        vault.SetGenesis(request.BlockHeight, request.BlockHeight);
                    }

                    var vaultUpdates = await _mediator.Send(new MakeVaultCommand(vault));
                    if (vaultUpdates == 0) return false;
                }

                // Update mining governance if applicable
                await _mediator.Send(new CreateMiningGovernanceCommand(governance.Address, request.BlockHeight, true));

                // Process the distributed token total supply updates
                if (request.BlockHeight >= token.ModifiedBlock)
                {
                    token.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);
                    await _mediator.Send(new MakeTokenCommand(token));
                }

                // First period, index mining governance nominations
                if (request.Log.PeriodIndex == 0)
                {
                    var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(token.Id));
                    await InitializeNominations(miningGovernance.Id, miningGovernance.Address, request.BlockHeight);
                }

                var latestDistribution = await _mediator.Send(new RetrieveLatestTokenDistributionQuery(findOrThrow: false));

                if (latestDistribution != null && latestDistribution.PeriodIndex >= request.Log.PeriodIndex)
                {
                    return true;
                }

                var distribution = new TokenDistribution(token.Id, request.Log.VaultAmount, request.Log.MiningAmount, (int)request.Log.PeriodIndex,
                                                         request.BlockHeight, request.Log.NextDistributionBlock, request.BlockHeight);

                return await _mediator.Send(new MakeTokenDistributionCommand(distribution), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(DistributionLog)}");

                return false;
            }
        }

        private async Task InitializeNominations(long governanceId, Address miningGovernance, ulong blockHeight)
        {
            var nominatedPools = await _mediator.Send(new RetrieveCirrusMiningGovernanceNominationsQuery(miningGovernance, blockHeight));

            var nominatedLiquidityPools = await Task.WhenAll(
                nominatedPools.Select(nomination => _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(nomination.LiquidityPool))));

            var nominatedMiningPools = await Task.WhenAll(
                nominatedLiquidityPools.Select(pool => _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id))));

            var nominations = nominatedMiningPools.Select(miningPool => new MiningGovernanceNomination(governanceId,
                                                                                                       miningPool.LiquidityPoolId,
                                                                                                       miningPool.Id,
                                                                                                       true,
                                                                                                       1,
                                                                                                       blockHeight));

            await Task.WhenAll(
                nominations.Select(nomination => _mediator.Send(new MakeMiningGovernanceNominationCommand(nomination))));
        }
    }
}
