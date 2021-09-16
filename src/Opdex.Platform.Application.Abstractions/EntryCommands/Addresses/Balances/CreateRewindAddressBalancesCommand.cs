using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances
{
    /// <summary>
    /// Create a rewind address balances command to refresh balances back at a specified block in time.
    /// </summary>
    public class CreateRewindAddressBalancesCommand : IRequest<bool>
    {
        /// <summary>
        /// Create the create rewind address balances command.
        /// </summary>
        /// <param name="rewindHeight">The block height to rewind applicable address balance records back to.</param>
        public CreateRewindAddressBalancesCommand(ulong rewindHeight)
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
