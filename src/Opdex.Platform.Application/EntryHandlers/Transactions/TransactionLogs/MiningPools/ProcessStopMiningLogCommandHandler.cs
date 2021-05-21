using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools
{
    public class ProcessStopMiningLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessStopMiningLogCommand, bool>
    {
        private readonly ILogger<ProcessStopMiningLogCommandHandler> _logger;

        public ProcessStopMiningLogCommandHandler(IMediator mediator, ILogger<ProcessStopMiningLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStopMiningLogCommand request, CancellationToken cancellationToken)
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

                var addressMiningQuery = new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, request.Log.Miner, findOrThrow: true);
                var miningBalance = await _mediator.Send(addressMiningQuery, CancellationToken.None);
                
                if (request.BlockHeight < miningBalance.ModifiedBlock)
                {
                    return true;
                }
                
                miningBalance.ResetBalance(request.Log, request.BlockHeight);

                var miningBalanceCommand = new MakeAddressMiningCommand(miningBalance);
                var miningBalanceId = await _mediator.Send(miningBalanceCommand, CancellationToken.None);

                return miningBalanceId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StopMiningLog)}");
               
                return false;
            }
        }
    }
}