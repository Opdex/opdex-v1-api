using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking
{
    /// <summary>
    /// Command to rewind staking address details
    /// </summary>
    public class CreateRewindStakingPositionsCommand : IRequest<bool>
    {
        /// <summary>
        /// Creates a command to rewind staking address details
        /// </summary>
        /// <param name="rewindHeight">The block height to rewind applicable records back to.</param>
        public CreateRewindStakingPositionsCommand(ulong rewindHeight)
        {
            if (rewindHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
            }

            RewindHeight = rewindHeight;
        }

        public ulong RewindHeight { get; }
    }
}
