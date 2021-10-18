using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
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
                // We are only going to process balances of tokens we know about
                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: false));
                if (token == null)
                {
                    return false;
                }

                // Update sender balance
                await _mediator.Send(new CreateAddressBalanceCommand(request.Log.From, request.Log.Contract, request.BlockHeight));

                // Update receiver balance
                await _mediator.Send(new CreateAddressBalanceCommand(request.Log.To, request.Log.Contract, request.BlockHeight));

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
