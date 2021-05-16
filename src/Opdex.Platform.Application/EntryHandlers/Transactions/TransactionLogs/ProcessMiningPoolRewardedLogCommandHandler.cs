using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessMiningPoolRewardedLogCommandHandler : IRequestHandler<ProcessMiningPoolRewardedLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessMiningPoolRewardedLogCommandHandler> _logger;

        public ProcessMiningPoolRewardedLogCommandHandler(IMediator mediator, ILogger<ProcessMiningPoolRewardedLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMiningPoolRewardedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MiningPoolRewardedLog)}");
               
                return false;
            }
        }
    }
}