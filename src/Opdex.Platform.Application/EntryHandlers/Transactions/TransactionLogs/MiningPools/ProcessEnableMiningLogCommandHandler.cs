using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools
{
    public class ProcessEnableMiningLogCommandHandler : IRequestHandler<ProcessEnableMiningLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessEnableMiningLogCommandHandler> _logger;

        public ProcessEnableMiningLogCommandHandler(IMediator mediator, ILogger<ProcessEnableMiningLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessEnableMiningLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.Log.Contract, findOrThrow: false));
                if (miningPool == null) return false;

                if (request.BlockHeight < miningPool.ModifiedBlock)
                {
                    return true;
                }

                miningPool.EnableMining(request.Log, request.BlockHeight);

                return  await _mediator.Send(new MakeMiningPoolCommand(miningPool, request.BlockHeight, refreshRewardPerLpt: true)) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(EnableMiningLog)}");

                return false;
            }
        }
    }
}
