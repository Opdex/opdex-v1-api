using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessDistributionLogCommandHandler : IRequestHandler<ProcessDistributionLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessDistributionLogCommandHandler> _logger;

        public ProcessDistributionLogCommandHandler(IMediator mediator, ILogger<ProcessDistributionLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessDistributionLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var initialDistribution = request.Log.PeriodIndex == 0;

                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: false));
                if (token == null) return false;

                var vault = await _mediator.Send(new RetrieveVaultByTokenIdQuery(token.Id, findOrThrow: false));
                if (vault == null) return false;

                var governance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(token.Id, findOrThrow: false));
                if (governance == null) return false;

                var latestDistribution = await _mediator.Send(new RetrieveLatestTokenDistributionQuery(findOrThrow: false));
                if (latestDistribution != null && latestDistribution.PeriodIndex >= request.Log.PeriodIndex)
                {
                    return true;
                }

                var distribution = new TokenDistribution(token.Id, request.Log.VaultAmount, request.Log.MiningAmount, (int)request.Log.PeriodIndex,
                                                         request.BlockHeight, request.Log.NextDistributionBlock, request.BlockHeight);

                var madeDistribution = await _mediator.Send(new MakeTokenDistributionCommand(distribution));
                if (!madeDistribution)  return false;

                // Process the distributed token total supply updates
                if (request.BlockHeight >= token.ModifiedBlock)
                {
                    token.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);

                    var tokenId = await _mediator.Send(new MakeTokenCommand(token, request.BlockHeight));

                    if (tokenId == 0)
                    {
                        _logger.LogError($"Unknown error updating token {token.Id} details during distribution");
                    }
                }

                // Update mining/nominations on the initial distribution
                if (initialDistribution)
                {
                    var governanceId = await _mediator.Send(new MakeMiningGovernanceCommand(governance, request.BlockHeight,
                                                                                            refreshMiningPoolReward: true,
                                                                                            refreshNominationPeriodEnd: true));
                    if (governanceId == 0)
                    {
                        _logger.LogError($"Unknown error updating governance {governance.Id} details during initial distribution");
                    }

                    var updatedNominations = await _mediator.Send(new MakeGovernanceNominationsCommand(governance, request.BlockHeight));
                    if (!updatedNominations)
                    {
                        _logger.LogError($"Unknown error updating governance {governance.Id} nominations during initial distribution");
                    }
                }

                // try to process vault balances
                var vaultResult = await _mediator.Send(new CreateAddressBalanceCommand(vault.Address, token.Address, request.BlockHeight));
                if (vaultResult == 0)
                {
                    _logger.LogError($"Unknown error updating vault {vault.Id} balance during distribution");
                }

                // try to process governance balances
                var governanceResult = await _mediator.Send(new CreateAddressBalanceCommand(governance.Address, token.Address, request.BlockHeight));
                if (governanceResult == 0)
                {
                    _logger.LogError($"Unknown error updating governance {governance.Id} balance during distribution");
                }

                // try to refresh the vault
                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var vaultId = await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight, refreshSupply: true,
                                                                            refreshGenesis: initialDistribution));
                    if (vaultId == 0)
                    {
                        _logger.LogError($"Unknown error updating vault {vault.Id} details during distribution");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(DistributionLog)}");

                return false;
            }
        }
    }
}
