using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessSwapLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessSwapLogCommand, bool>
    {
        private readonly ILogger<ProcessSwapLogCommandHandler> _logger;

        public ProcessSwapLogCommandHandler(IMediator mediator, ILogger<ProcessSwapLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessSwapLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await MakeTransactionLog(request.Log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(SwapLog)}");

                return false;
            }
        }
    }
}
