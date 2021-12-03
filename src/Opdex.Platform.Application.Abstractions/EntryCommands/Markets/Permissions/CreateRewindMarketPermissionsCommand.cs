using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;

/// <summary>
/// Create rewind market permissions command to refresh wallet permission data based at a certain block height.
/// </summary>
public class CreateRewindMarketPermissionsCommand : IRequest<bool>
{
    /// <summary>
    /// Create the create rewind market permissions command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind to.</param>
    public CreateRewindMarketPermissionsCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}