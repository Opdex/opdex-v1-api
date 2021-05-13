using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessStopStakingLogCommandHandler : IRequestHandler<ProcessStopStakingLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStopStakingLogCommandHandler> _logger;

        public ProcessStopStakingLogCommandHandler(IMediator mediator, ILogger<ProcessStopStakingLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStopStakingLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the users staking amount
                
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StopStakingLog)}");
               
                return false;
            }
        }
    }
}