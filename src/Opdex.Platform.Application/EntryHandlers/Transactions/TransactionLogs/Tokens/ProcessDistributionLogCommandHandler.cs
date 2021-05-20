using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
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
                
                // Todo: Fix this comment below
                // 1_971_000 will be prod - 1 year, but will need to ask the contract for this for local/testnet
                var nextDistributionBlock = vault.Genesis + (1_971_000ul * nextPeriodIndex);
                
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
    }
}