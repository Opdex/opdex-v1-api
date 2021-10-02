using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands
{
    /// <summary>
    /// Create the rewind snapshots command to take in a block height to be used to recalculate existing snapshots affected by the rewind.
    /// </summary>
    public class CreateRewindSnapshotsCommand : IRequest<bool>
    {
        /// <summary>
        /// Create the create rewind snapshots command.
        /// </summary>
        /// <param name="rewindHeight">The block height to rewind to.</param>
        public CreateRewindSnapshotsCommand(ulong rewindHeight)
        {
            if (rewindHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
            }

            RewindHeight = rewindHeight;
        }

        public ulong RewindHeight { get; }
    }
}
