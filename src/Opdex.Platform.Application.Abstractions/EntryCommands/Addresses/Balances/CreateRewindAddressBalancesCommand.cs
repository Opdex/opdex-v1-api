using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances
{
    public class CreateRewindAddressBalancesCommand : IRequest<bool>
    {
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
