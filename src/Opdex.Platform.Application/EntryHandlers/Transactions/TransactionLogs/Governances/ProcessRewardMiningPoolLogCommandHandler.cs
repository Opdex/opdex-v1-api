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
                var governance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Log.Contract, findOrThrow: true));

                var updateId = await _mediator.Send(new MakeMiningGovernanceCommand(governance, request.BlockHeight,
                                                                                    refreshMiningPoolReward: true,
                                                                                    refreshNominationPeriodEnd: true,
                                                                                    refreshMiningPoolsFunded: true));

                return updateId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(RewardMiningPoolLog)}");

                return false;
            }
        }
    }
}
