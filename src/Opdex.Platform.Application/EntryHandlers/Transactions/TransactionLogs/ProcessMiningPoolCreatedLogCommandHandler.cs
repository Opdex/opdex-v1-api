using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessMiningPoolCreatedLogCommandHandler : IRequestHandler<ProcessMiningPoolCreatedLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessMiningPoolCreatedLogCommandHandler> _logger;

        public ProcessMiningPoolCreatedLogCommandHandler(IMediator mediator, ILogger<ProcessMiningPoolCreatedLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMiningPoolCreatedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.StakingPool), CancellationToken.None);
                var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(request.Log.MiningPool, pool.Id), CancellationToken.None);

                return miningPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MiningPoolCreatedLog)}");
               
                return false;
            }
        }
    }
}