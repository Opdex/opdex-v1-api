using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernance
{
    public class ProcessCreateMiningPoolLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCreateMiningPoolLogCommand, bool>
    {
        private readonly ILogger<ProcessCreateMiningPoolLogCommandHandler> _logger;

        public ProcessCreateMiningPoolLogCommandHandler(IMediator mediator, ILogger<ProcessCreateMiningPoolLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateMiningPoolLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var poolQuery = new RetrieveLiquidityPoolByAddressQuery(request.Log.StakingPool, findOrThrow: true);
                var pool = await _mediator.Send(poolQuery, CancellationToken.None);

                var miningPoolQuery = new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id, findOrThrow: false);
                var miningPool = await _mediator.Send(miningPoolQuery, CancellationToken.None);

                if (miningPool != null) return true;
                
                miningPool = new MiningPool(pool.Id, request.Log.Contract, request.BlockHeight);
                
                var miningPoolCommand = new MakeMiningPoolCommand(miningPool);
                var miningPoolId = await _mediator.Send(miningPoolCommand, CancellationToken.None);
                
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