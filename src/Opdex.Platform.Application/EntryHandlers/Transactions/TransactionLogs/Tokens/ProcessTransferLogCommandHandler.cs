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
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

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
                
                var liquidityPoolQuery = new RetrieveLiquidityPoolByAddressQuery(tokenAddress, findOrThrow: false);
                var liquidityPool = await _mediator.Send(liquidityPoolQuery, CancellationToken.None);
                var liquidityPoolId = liquidityPool?.Id ?? 0;

                var tokenQuery = new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: false);
                var token = await _mediator.Send(tokenQuery, CancellationToken.None);
                var tokenId = token?.Id ?? 0;

                if (token == null && liquidityPool == null)
                {
                    return false;
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
                var addressBalance = token != null
                    ? await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(tokenId, request.Log.From, findOrThrow: false), CancellationToken.None)
                    : await _mediator.Send(new RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQuery(liquidityPoolId, request.Log.From, findOrThrow: false), CancellationToken.None);
                
                addressBalance ??= new AddressBalance(tokenId, liquidityPoolId, request.Log.From, "0", request.BlockHeight);

                if (addressBalance.ModifiedBlock > request.BlockHeight)
                {
                    return true;
                }
                
                var tokenBalanceAddress = liquidityPool != null ? liquidityPool.Address : token.Address;
                var balanceQuery = new CallCirrusGetSrcTokenBalanceQuery(tokenBalanceAddress, request.Log.From);
                var balance = await _mediator.Send(balanceQuery, CancellationToken.None);

                addressBalance.SetBalance(balance, request.BlockHeight);
                    
                await _mediator.Send(new MakeAddressBalanceCommand(addressBalance), CancellationToken.None);

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