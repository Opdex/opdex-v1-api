using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessCollectStakingRewardsLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCollectStakingRewardsLogCommand, bool>
    {
        private readonly ILogger<ProcessCollectStakingRewardsLogCommandHandler> _logger;

        public ProcessCollectStakingRewardsLogCommandHandler(IMediator mediator, ILogger<ProcessCollectStakingRewardsLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCollectStakingRewardsLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CollectStakingRewardsLog)}");
               
                return false;
            }
        }
    }
}