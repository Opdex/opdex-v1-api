using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    public class CreateRewindVaultsCommand : IRequest<bool>
    {
        public CreateRewindVaultsCommand(ulong rewindHeight)
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
