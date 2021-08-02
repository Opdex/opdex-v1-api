using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessSetPendingMarketOwnershipLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessSetPendingMarketOwnershipLogCommand, bool>
    {
        private readonly ILogger<ProcessSetPendingMarketOwnershipLogCommandHandler> _logger;

        public ProcessSetPendingMarketOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessSetPendingMarketOwnershipLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessSetPendingMarketOwnershipLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await MakeTransactionLog(request.Log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(SetPendingMarketOwnershipLog)}");

                return false;
            }
        }
    }
}
