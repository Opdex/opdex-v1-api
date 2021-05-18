using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessTransferLogCommandHandler : IRequestHandler<ProcessTransferLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessTransferLogCommandHandler> _logger;

        public ProcessTransferLogCommandHandler(IMediator mediator, ILogger<ProcessTransferLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessTransferLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isAllowanceTransfer = request.Sender != request.Log.From;

                var tokenAddress = request.Log.Contract;
                
                // Check DB, is either a liquidity pool token or standard token
                long tokenId = 0;
                long liquidityPoolId = 0;
                
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(tokenAddress), CancellationToken.None);
                if (liquidityPool == null)
                {
                    var token = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress), CancellationToken.None);
                    tokenId = token.Id;
                }
                else
                {
                    liquidityPoolId = liquidityPool.Id;
                }
                
                if (isAllowanceTransfer)
                {
                    // Update allowance
                    // Todo: RetrieveCirrusSrcTokenAllowanceQuery
                    // Todo: RetrieveAddressBalanceByTokenIdAndOwnerAndSpenderQuery
                    // Todo: RetrieveAddressBalanceByLiquidityPoolIdAndOwnerAndSpenderQuery
                    // await _mediator.Send(new MakeAddressAllowanceCommand(), CancellationToken.None);
                }
                
                // Update owner balance
                var addressBalance = tokenId > 0 
                    ? await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(tokenId, request.Log.From), CancellationToken.None)
                    : await _mediator.Send(new RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQuery(liquidityPoolId, request.Log.From), CancellationToken.None);

                if (addressBalance.ModifiedBlock < request.BlockHeight)
                {
                    // Get/Set latest address balance
                    // Todo: RetrieveCirrusSrcTokenBalanceQuery
                    await _mediator.Send(new MakeAddressBalanceCommand(addressBalance), CancellationToken.None);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(TransferLog)}");
               
                return false;
            }
        }
    }
}