using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class ProcessGovernanceDeploymentTransactionCommandHandler : IRequestHandler<ProcessGovernanceDeploymentTransactionCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessGovernanceDeploymentTransactionCommandHandler> _logger;

        public ProcessGovernanceDeploymentTransactionCommandHandler(IMediator mediator, ILogger<ProcessGovernanceDeploymentTransactionCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProcessGovernanceDeploymentTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Todo: Evaluate, rewrite, and/or reword all of these types of comments, these processes should be idempotent
                // Hosted environments would have indexed transaction already, local would need to reach out to Cirrus to get receipt
                var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TxHash, findOrThrow: false)) ??
                                  await _mediator.Send(new RetrieveCirrusTransactionByHashQuery(request.TxHash));

                if (transaction == null)
                {
                    return Unit.Value;
                }

                // Hosted environments would not be null, local environments would be null
                var block = await _mediator.Send(new RetrieveBlockByHeightQuery(transaction.BlockHeight, findOrThrow: false));

                if (block == null)
                {
                    // Get transaction block hash
                    var blockHash = await _mediator.Send(new RetrieveCirrusBlockHashByHeightQuery(transaction.BlockHeight));

                    // Get block by hash
                    var blockReceiptDto = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockHash));

                    // Make block
                    await _mediator.Send(new MakeBlockCommand(blockReceiptDto.Height, blockReceiptDto.Hash,
                                                              blockReceiptDto.Time, blockReceiptDto.MedianTime));
                }

                // Insert Staking Token
                var tokenAddress = transaction.NewContractAddress;
                var stakingToken = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: false));

                var stakingTokenId = stakingToken?.Id ?? 0L;
                if (stakingToken == null)
                {
                    var summary = await _mediator.Send(new CallCirrusGetSrcTokenSummaryByAddressQuery(tokenAddress));

                    stakingToken = new Token(summary.Address, false, summary.Name, summary.Symbol, (int)summary.Decimals, summary.Sats,
                                             summary.TotalSupply, transaction.BlockHeight);

                    stakingTokenId = await _mediator.Send(new MakeTokenCommand(stakingToken));
                }

                // Get token summary
                var tokenSummary = await _mediator.Send(new RetrieveStakingTokenContractSummaryByAddressQuery(tokenAddress));

                // Get and/or create vault
                var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(tokenSummary.Vault, findOrThrow: false));

                if (vault == null)
                {
                    vault = new Vault(tokenSummary.Vault, stakingTokenId, transaction.From, transaction.BlockHeight, UInt256.Zero, transaction.BlockHeight);
                    await _mediator.Send(new MakeVaultCommand(vault));
                }

                // Get and/or create mining governance
                var governanceId = await _mediator.Send(new CreateMiningGovernanceCommand(tokenSummary.MiningGovernance, transaction.BlockHeight, isUpdate: false));

                if (transaction.Id == 0)
                {
                    await _mediator.Send(new MakeTransactionCommand(transaction));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure processing mined token and governance deployment.");
            }

            return Unit.Value;
        }
    }
}
