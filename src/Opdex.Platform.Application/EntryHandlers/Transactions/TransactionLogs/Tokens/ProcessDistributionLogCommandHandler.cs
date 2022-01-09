using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;

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

            var vault = await _mediator.Send(new RetrieveVaultGovernanceByTokenIdQuery(token.Id, findOrThrow: false));
            if (vault == null) return false;

            var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(token.Id, findOrThrow: false));
            if (miningGovernance == null) return false;

            var latestDistribution = await _mediator.Send(new RetrieveLatestTokenDistributionByTokenIdQuery(token.Id, findOrThrow: false));
            if (latestDistribution != null && latestDistribution.PeriodIndex >= request.Log.PeriodIndex)
            {
                return true;
            }

            var distribution = new TokenDistribution(token.Id, request.Log.VaultAmount, request.Log.MiningAmount, (int)request.Log.PeriodIndex,
                                                     request.BlockHeight, request.Log.NextDistributionBlock, request.BlockHeight);

            var madeDistribution = await _mediator.Send(new MakeTokenDistributionCommand(distribution));
            if (!madeDistribution) return false;

            // Process the distributed token total supply updates
            if (request.BlockHeight >= token.ModifiedBlock)
            {
                token.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);

                var tokenId = await _mediator.Send(new MakeTokenCommand(token, request.BlockHeight));

                if (tokenId == 0)
                {
                    _logger.LogWarning($"Unknown error updating token {token.Id} details during distribution");
                }
            }

            // Update mining/nominations on the initial distribution
            if (initialDistribution)
            {
                var miningGovernanceId = await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance, request.BlockHeight,
                                                                                              refreshMiningPoolReward: true,
                                                                                              refreshNominationPeriodEnd: true));
                if (miningGovernanceId == 0)
                {
                    _logger.LogWarning($"Unknown error updating mining governance {miningGovernance.Id} details during initial distribution");
                }

                var updatedNominations = await _mediator.Send(new MakeMiningGovernanceNominationsCommand(miningGovernance, request.BlockHeight));
                if (!updatedNominations)
                {
                    _logger.LogWarning($"Unknown error updating mining governance {miningGovernance.Id} nominations during initial distribution");
                }
            }

            // try to process vault balances
            var vaultResult = await _mediator.Send(new CreateAddressBalanceCommand(vault.Address, token.Address, request.BlockHeight));
            if (vaultResult == 0)
            {
                _logger.LogWarning($"Unknown error updating vault {vault.Id} balance during distribution");
            }

            // try to process miningGovernance balances
            var miningGovernanceResult = await _mediator.Send(new CreateAddressBalanceCommand(miningGovernance.Address, token.Address, request.BlockHeight));
            if (miningGovernanceResult == 0)
            {
                _logger.LogWarning($"Unknown error updating mining governance {miningGovernance.Id} balance during distribution");
            }

            // try to refresh the vault
            if (request.BlockHeight >= vault.ModifiedBlock)
            {
                var vaultId = await _mediator.Send(new MakeVaultGovernanceCommand(vault, request.BlockHeight,
                                                                                  refreshUnassignedSupply: true));
                if (vaultId == 0)
                {
                    _logger.LogWarning($"Unknown error updating vault {vault.Id} details during distribution");
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
