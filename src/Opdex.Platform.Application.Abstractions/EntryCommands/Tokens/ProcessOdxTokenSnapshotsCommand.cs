using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens
{
    public class ProcessOdxTokenSnapshotsCommand : IRequest<Unit>
    {
        public ProcessOdxTokenSnapshotsCommand(DateTime blockTime)
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