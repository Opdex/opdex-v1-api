using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

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

                var initialDistribution = request.Log.PeriodIndex == 0;

                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: true));
                var vault = await _mediator.Send(new RetrieveVaultByTokenIdQuery(token.Id, findOrThrow: true));
                var governance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(token.Id, findOrThrow: true));

                // process vault balances
                var vaultResult = await _mediator.Send(new CreateAddressBalanceCommand(vault.Address, token.Address, request.BlockHeight));
                if (vaultResult <= 0) return false;

                // process governance balances
                var governanceResult = await _mediator.Send(new CreateAddressBalanceCommand(governance.Address, token.Address, request.BlockHeight));
                if (governanceResult <= 0) return false;

                // Refresh the vault
                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var vaultUpdates = await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight, refreshSupply: true,
                                                                                 refreshGenesis: initialDistribution));
                    if (vaultUpdates <= 0) return false;
                }

                // Initial distribution only, update governance and create initial nominated liquidity pools
                if (initialDistribution)
                {
                    await _mediator.Send(new CreateMiningGovernanceCommand(governance.Address, request.BlockHeight, true));
                    await _mediator.Send(new CreateGovernanceNominationsCommand(governance.Address, request.BlockHeight));
                }

                // Process the distributed token total supply updates
                if (request.BlockHeight >= token.ModifiedBlock)
                {
                    token.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);
                    await _mediator.Send(new MakeTokenCommand(token));
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
    }
}
