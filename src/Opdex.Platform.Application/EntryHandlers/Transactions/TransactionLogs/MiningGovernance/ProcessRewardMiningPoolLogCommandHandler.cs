using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessRewardMiningPoolLogCommandHandler : IRequestHandler<ProcessRewardMiningPoolLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessRewardMiningPoolLogCommandHandler> _logger;

        public ProcessRewardMiningPoolLogCommandHandler(IMediator mediator, ILogger<ProcessRewardMiningPoolLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessRewardMiningPoolLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the amount the user was using to mine
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(RewardMiningPoolLog)}");
               
                return false;
            }
        }
    }
}