using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Governances
{
    public class ProcessRewardMiningPoolLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessRewardMiningPoolLogCommand, bool>
    {
        private readonly ILogger<ProcessRewardMiningPoolLogCommandHandler> _logger;

        public ProcessRewardMiningPoolLogCommandHandler(IMediator mediator, ILogger<ProcessRewardMiningPoolLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessRewardMiningPoolLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var governanceId = await _mediator.Send(new CreateMiningGovernanceCommand(request.Log.Contract, request.BlockHeight, isUpdate: true));

                return governanceId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(RewardMiningPoolLog)}");

                return false;
            }
        }
    }
}
