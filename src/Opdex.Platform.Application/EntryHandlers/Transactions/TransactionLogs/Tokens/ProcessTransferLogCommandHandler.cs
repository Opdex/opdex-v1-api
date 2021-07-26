using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Addresses;
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

                var tokenAddress = request.Log.Contract;

                var tokenQuery = new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow: true);
                var token = await _mediator.Send(tokenQuery, CancellationToken.None);

                // Note: This is technically not a very efficient way of checking if a TransferLog is an allowance transaction, however, there is no
                // clean way of doing so. There are no logs or indicators to accurately know. If the sender is not equal to the token transfer's
                // From address, check allowances both ways.
                var isPotentialAllowanceTransfer = request.Sender != request.Log.From;
                if (isPotentialAllowanceTransfer)
                {
                    await TryUpdateAddressAllowance(token, request.Log.From, request.Sender, request.BlockHeight);
                    await TryUpdateAddressAllowance(token, request.Sender, request.Log.From, request.BlockHeight);
                }

                // Update sender balance
                await TryUpdateAddressBalance(token, request.Log.From, request.BlockHeight);
                // Update receiver balance
                await TryUpdateAddressBalance(token, request.Log.To, request.BlockHeight);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(TransferLog)}");

                return false;
            }
        }

        private async Task TryUpdateAddressAllowance(Token token, string owner, string spender, ulong blockHeight)
        {
            try
            {
                var allowance = await _mediator.Send(new RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(token.Id, owner, spender, findOrThrow: false));

                if (allowance != null && allowance.ModifiedBlock >= blockHeight)
                {
                    return;
                }

                allowance ??= new AddressAllowance(token.Id, owner, spender, "0", blockHeight);

                var allowanceAmount = await _mediator.Send(new CallCirrusGetSrcTokenAllowanceQuery(token.Address, owner, spender));

                allowance.SetAllowance(allowanceAmount, blockHeight);

                await _mediator.Send(new MakeAddressAllowanceCommand(allowance), CancellationToken.None);
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["Token"] = token.Name,
                    ["Owner"] = owner,
                    ["Spender"] = spender
                }))
                {
                    _logger.LogError(ex, $"Unexpected error updating address allowance.");
                }
            }
        }

        private async Task TryUpdateAddressBalance(Token token, string address, ulong blockHeight)
        {
            try
            {
                var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(address,
                                                                                                         tokenAddress: token.Address,
                                                                                                         findOrThrow: false));

                if (addressBalance != null && addressBalance.ModifiedBlock >= blockHeight)
                {
                    return;
                }

                addressBalance ??= new AddressBalance(token.Id, address, "0", blockHeight);

                var balance = await _mediator.Send(new CallCirrusGetSrcTokenBalanceQuery(token.Address, address));

                addressBalance.SetBalance(balance, blockHeight);

                await _mediator.Send(new MakeAddressBalanceCommand(addressBalance), CancellationToken.None);
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["Token"] = token.Name,
                    ["Address"] = address
                }))
                {
                    _logger.LogError(ex, $"Unexpected error updating address balance.");
                }
            }
        }
    }
}
