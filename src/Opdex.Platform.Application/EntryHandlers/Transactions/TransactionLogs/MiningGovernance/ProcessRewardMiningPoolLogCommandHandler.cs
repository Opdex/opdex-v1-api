using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernance;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernance;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernance
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

                var miningGovernanceQuery = new RetrieveMiningGovernanceQuery(findOrThrow: true);
                var miningGovernance = await _mediator.Send(miningGovernanceQuery, CancellationToken.None);

                var isNewer = request.BlockHeight < miningGovernance.ModifiedBlock;
                if (isNewer && miningGovernance.Id != 0)
                {
                    return false;
                }

                var miningGovernanceSummaryQuery = new RetrieveMiningGovernanceContractSummaryByAddressQuery(miningGovernance.Address);
                var miningGovernanceSummary = await _mediator.Send(miningGovernanceSummaryQuery, CancellationToken.None);

                miningGovernance.Update(miningGovernanceSummary, request.BlockHeight);

                return await _mediator.Send(new MakeMiningGovernanceCommand(miningGovernance)) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(RewardMiningPoolLog)}");

                return false;
            }
        }
    }
}
