using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessStartMiningLogCommandHandler : IRequestHandler<ProcessStartMiningLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStartMiningLogCommandHandler> _logger;

        public ProcessStartMiningLogCommandHandler(IMediator mediator, ILogger<ProcessStartMiningLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStartMiningLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the amount the user was using to mine
                
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StartMiningLog)}");
               
                return false;
            }
        }
    }
}