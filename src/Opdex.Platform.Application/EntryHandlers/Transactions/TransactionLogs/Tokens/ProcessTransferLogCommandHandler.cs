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
    public class ProcessTransferLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessTransferLogCommand, bool>
    {
        private readonly ILogger<ProcessTransferLogCommandHandler> _logger;

        public ProcessTransferLogCommandHandler(IMediator mediator, ILogger<ProcessTransferLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessTransferLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var isAllowanceTransfer = request.Sender != request.Log.From;
                var tokenAddress = request.Log.Contract;
                
                long tokenId = 0;
                long liquidityPoolId = 0;
                
                var liquidityPoolQuery = new RetrieveLiquidityPoolByAddressQuery(tokenAddress, findOrThrow: false);
                var liquidityPool = await _mediator.Send(liquidityPoolQuery, CancellationToken.None);
                
                if (liquidityPool != null)
                {
                    liquidityPoolId = liquidityPool.Id;
                }
                else
                {
                    var tokenQuery = new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: true);
                    var token = await _mediator.Send(tokenQuery, CancellationToken.None);
                    
                    tokenId = token.Id;
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