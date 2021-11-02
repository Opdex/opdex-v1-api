using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots
{
    public class CreateCrsTokenSnapshotsCommand : IRequest<bool>
    {
        public CreateCrsTokenSnapshotsCommand(DateTime blockTime, ulong blockHeight)
        {
            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime), "Block time must be set.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockTime = blockTime;
            BlockHeight = blockHeight;
        }

        public DateTime BlockTime { get; }
        public ulong BlockHeight { get; }
    }
}
