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
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
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

                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract));

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

        private async Task TryUpdateAddressBalance(Token token, Address address, ulong blockHeight)
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

                addressBalance ??= new AddressBalance(token.Id, address, UInt256.Zero, blockHeight);

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
