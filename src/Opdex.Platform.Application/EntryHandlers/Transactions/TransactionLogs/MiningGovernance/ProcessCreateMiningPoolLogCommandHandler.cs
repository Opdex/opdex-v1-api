using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessCreateMiningPoolLogCommandHandler : IRequestHandler<ProcessCreateMiningPoolLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessCreateMiningPoolLogCommandHandler> _logger;

        public ProcessCreateMiningPoolLogCommandHandler(IMediator mediator, ILogger<ProcessCreateMiningPoolLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateMiningPoolLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.StakingPool), CancellationToken.None);
                var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(request.Log.MiningPool, pool.Id), CancellationToken.None);

                return miningPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateMiningPoolLog)}");
               
                return false;
            }
        }
    }
}