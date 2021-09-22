using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools
{
    public class ProcessMineLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessMineLogCommand, bool>
    {
        private readonly ILogger<ProcessMineLogCommandHandler> _logger;

        public ProcessMineLogCommandHandler(IMediator mediator, ILogger<ProcessMineLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMineLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.Log.Contract, findOrThrow: true));

                var miningBalance = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, request.Log.Miner, findOrThrow: false))
                                    ?? new AddressMining(miningPool.Id, request.Log.Miner, UInt256.Zero, request.BlockHeight);

                if (request.BlockHeight < miningBalance.ModifiedBlock)
                {
                    return false;
                }

                miningBalance.SetBalance(request.Log.MinerBalance, request.BlockHeight);

                var miningBalanceId = await _mediator.Send(new MakeAddressMiningCommand(miningBalance));

                if (miningBalanceId <= 0)
                {
                    return false;
                }

                var miningPoolId = await _mediator.Send(new MakeMiningPoolCommand(miningPool, request.BlockHeight, refreshRewardPerLpt: true));

                return miningPoolId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MineLog)}");

                return false;
            }
        }
    }
}
