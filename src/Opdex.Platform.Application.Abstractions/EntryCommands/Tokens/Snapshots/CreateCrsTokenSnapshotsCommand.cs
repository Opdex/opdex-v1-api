using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots
{
    public class CreateCrsTokenSnapshotsCommand : IRequest<Unit>
    {
        public CreateCrsTokenSnapshotsCommand(DateTime blockTime, ulong blockHeight)
        {
            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime));
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
