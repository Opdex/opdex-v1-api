using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Create rewind vaults command to refresh vault data based at a certain block height.
/// </summary>
public class CreateRewindVaultsCommand : IRequest<bool>
{
    /// <summary>
    /// Create the create rewind vaults command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind to.</param>
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