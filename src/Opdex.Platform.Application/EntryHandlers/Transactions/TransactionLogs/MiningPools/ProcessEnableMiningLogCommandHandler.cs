using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools
{
    public class ProcessEnableMiningLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessEnableMiningLogCommand, bool>
    {
        private readonly ILogger<ProcessEnableMiningLogCommandHandler> _logger;

        public ProcessEnableMiningLogCommandHandler(IMediator mediator, ILogger<ProcessEnableMiningLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessEnableMiningLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var miningPoolQuery = new RetrieveMiningPoolByAddressQuery(request.Log.Contract, findOrThrow: true);
                var miningPool = await _mediator.Send(miningPoolQuery, CancellationToken.None);

                if (miningPool.ModifiedBlock <= request.BlockHeight)
                {
                    miningPool.EnableMiningPool(request.Log, request.BlockHeight);
                }

                var miningPoolCommand = new MakeMiningPoolCommand(miningPool);
                var miningPoolId = await _mediator.Send(miningPoolCommand, CancellationToken.None);
                
                return miningPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(EnableMiningLog)}");
               
                return false;
            }
        }
    }
}