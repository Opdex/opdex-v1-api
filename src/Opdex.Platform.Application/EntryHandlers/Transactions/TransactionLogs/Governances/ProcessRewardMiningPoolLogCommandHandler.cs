using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
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

                var miningGovernanceQuery = new RetrieveMiningGovernanceQuery(findOrThrow: true);
                var miningGovernance = await _mediator.Send(miningGovernanceQuery, CancellationToken.None);

                var miningGovernanceSummaryQuery = new RetrieveMiningGovernanceContractSummaryByAddressQuery(miningGovernance.Address);
                var miningGovernanceSummary = await _mediator.Send(miningGovernanceSummaryQuery, CancellationToken.None);

                var isNewer = request.BlockHeight < miningGovernance.ModifiedBlock;
                if (isNewer && miningGovernance.Id != 0)
                {
                    return false;
                }

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
