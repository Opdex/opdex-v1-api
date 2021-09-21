using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Governances
{
    /// <summary>
    /// Create the rewind mining governances and nominations command where values are refreshed
    /// back to a specified block height.
    /// </summary>
    public class CreateRewindMiningGovernancesAndNominationsCommand : IRequest<bool>
    {
        /// <summary>
        /// Constructor to create a rewind mining governances and nominations command.
        /// </summary>
        /// <param name="rewindHeight">The block height to rewind to.</param>
        public CreateRewindMiningGovernancesAndNominationsCommand(ulong rewindHeight)
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
