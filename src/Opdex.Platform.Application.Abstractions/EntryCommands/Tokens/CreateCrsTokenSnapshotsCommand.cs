using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens
{
    public class CreateCrsTokenSnapshotsCommand : IRequest<Unit>
    {
        public CreateCrsTokenSnapshotsCommand(DateTime blockTime)
        {
            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime));
            }

            BlockTime = blockTime;
        }
        
        public DateTime BlockTime { get; }
    }
}