using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessReservesLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessReservesLogCommand, bool>
    {
        private readonly ILogger<ProcessReservesLogCommandHandler> _logger;

        public ProcessReservesLogCommandHandler(IMediator mediator, ILogger<ProcessReservesLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessReservesLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await MakeTransactionLog(request.Log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ReservesLog)}");

                return false;
            }
        }
    }
}
