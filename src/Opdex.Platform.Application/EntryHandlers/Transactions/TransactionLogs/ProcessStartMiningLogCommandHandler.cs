using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessStartMiningLogCommandHandler : IRequestHandler<ProcessStartMiningLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessStartMiningLogCommandHandler> _logger;

        public ProcessStartMiningLogCommandHandler(IMediator mediator, ILogger<ProcessStartMiningLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessStartMiningLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract), CancellationToken.None);
                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id), CancellationToken.None);
                
                var miningBalance = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, request.Log.Miner), CancellationToken.None) 
                                    ?? new AddressMining(miningPool.Id, request.Log.Miner, request.Log.Amount, request.BlockHeight, request.BlockHeight);

                if (request.BlockHeight <= miningBalance.ModifiedBlock)
                {
                    return true;
                }
                
                if (miningBalance.Id != 0)
                {
                    miningBalance.SetBalance(request.Log, request.BlockHeight);
                }
                
                var miningBalanceId = await _mediator.Send(new MakeAddressMiningCommand(miningBalance), CancellationToken.None);

                return miningBalanceId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(StartMiningLog)}");
               
                return false;
            }
        }
    }
}