using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools
{
    public class ProcessCollectMiningRewardsLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCollectMiningRewardsLogCommand, bool>
    {
        private readonly ILogger<ProcessCollectMiningRewardsLogCommandHandler> _logger;

        public ProcessCollectMiningRewardsLogCommandHandler(IMediator mediator, ILogger<ProcessCollectMiningRewardsLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCollectMiningRewardsLogCommand request, CancellationToken cancellationToken)
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
                _logger.LogError(ex, $"Failure processing {nameof(CollectMiningRewardsLog)}");
               
                return false;
            }
        }
    }
}