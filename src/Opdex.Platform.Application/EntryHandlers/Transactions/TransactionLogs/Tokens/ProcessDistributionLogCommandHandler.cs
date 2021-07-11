using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.ODX;
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

                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: true));
                var vault = await _mediator.Send(new RetrieveVaultByTokenIdQuery(token.Id, findOrThrow: true));

                // process address balances
                var vaultBalance = await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(token.Id, vault.Address, findOrThrow: false));
                if (vaultBalance is null || request.BlockHeight >= vaultBalance.ModifiedBlock)
                {
                    vaultBalance ??= new AddressBalance(token.Id, vault.Address, request.Log.VaultAmount, request.BlockHeight);
                    // TODO: update balance for an existing record
                    await _mediator.Send(new MakeAddressBalanceCommand(vaultBalance));
                }

                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var totalSupply = await _mediator.Send(new RetrieveCirrusVaultTotalSupplyQuery(vault.Address, request.BlockHeight));

                    vault.SetUnassignedSupply(totalSupply, request.BlockHeight);

                    var vaultUpdates = await _mediator.Send(new MakeVaultCommand(vault));
                    if (vaultUpdates == 0) return false;
                }

                var periodIndex = request.Log.PeriodIndex;

                var latestDistributionQuery = new RetrieveLatestTokenDistributionQuery(findOrThrow: false);
                var latestDistribution = await _mediator.Send(latestDistributionQuery, CancellationToken.None);

                if (latestDistribution != null && latestDistribution.PeriodIndex >= periodIndex)
                {
                    return true;
                }

                var blockHeight = request.BlockHeight;
                var vaultAmount = request.Log.VaultAmount;
                var miningAmount = request.Log.MiningAmount;
                var nextPeriodIndex = periodIndex + 1;

                // Get the period duration (per year) from the smart contract dynamically
                var periodDurationRequest = new RetrieveCirrusLocalCallSmartContractQuery(token.Address, "get_PeriodDuration");
                var periodDurationSerialized = await _mediator.Send(periodDurationRequest, CancellationToken.None);
                var periodDuration = periodDurationSerialized.DeserializeValue<ulong>();

                var nextDistributionBlock = vault.Genesis + (periodDuration * nextPeriodIndex);
                var distribution = new TokenDistribution(vaultAmount, miningAmount, (int)periodIndex, blockHeight, nextDistributionBlock, blockHeight);

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
