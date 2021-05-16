using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessBurnLogCommandHandler : IRequestHandler<ProcessBurnLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessBurnLogCommandHandler> _logger;

        public ProcessBurnLogCommandHandler(IMediator mediator, ILogger<ProcessBurnLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessBurnLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the owner LP token balances
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(BurnLog)}");
               
                return false;
            }
        }
    }
}