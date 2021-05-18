using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessStopStakingLogCommand : IRequest<bool>
    {
        public ProcessStopStakingLogCommand(TransactionLog log, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(blockHeight));
            }

            Log = log as StopStakingLog ?? throw new ArgumentNullException(nameof(log));
            BlockHeight = blockHeight;
        }

        public StopStakingLog Log { get; }
        public ulong BlockHeight { get; }
    }
}