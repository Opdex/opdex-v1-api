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

                var tokenQuery = new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: false);
                var token = await _mediator.Send(tokenQuery, CancellationToken.None);
                var tokenId = token?.Id ?? 0;

                if (token == null)
                {
                    return false;
                }

                if (isAllowanceTransfer)
                {
                    // Update spender allowance (request.Sender)
                }

                // Update sender balance
                await TryUpdateAddressBalance(token, tokenId, request.Log.From, request.BlockHeight);

                // Update receiver balance
                await TryUpdateAddressBalance(token, tokenId, request.Log.To, request.BlockHeight);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(TransferLog)}");

                return false;
            }
        }

        private async Task TryUpdateAddressBalance(Token token,long tokenId, string wallet, ulong blockHeight)
        {
            try
            {
                var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(tokenId, wallet, findOrThrow: false));

                addressBalance ??= new AddressBalance(tokenId, wallet, "0", blockHeight);

                var isNewer = blockHeight < addressBalance.ModifiedBlock;
                if (isNewer && addressBalance.Id != 0)
                {
                    return;
                }

                var balance = await _mediator.Send(new CallCirrusGetSrcTokenBalanceQuery(token.Address, wallet));

                addressBalance.SetBalance(balance, blockHeight);

                await _mediator.Send(new MakeAddressBalanceCommand(addressBalance), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update address balance for {wallet}");
            }
        }
    }
}