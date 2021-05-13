using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessStopMiningLogCommandHandler : IRequestHandler<ProcessStopMiningLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStopMiningLogCommandHandler> _logger;

        public ProcessStopMiningLogCommandHandler(IMediator mediator, ILogger<ProcessStopMiningLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStopMiningLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the amount the user was using to mine
                
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StopMiningLog)}");
               
                return false;
            }
        }
    }
}