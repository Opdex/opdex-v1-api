using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;

/// <summary>
/// Create rewind mining pools command to refresh mining pool data based at a certain block height.
/// </summary>
public class CreateRewindMiningPoolsCommand : IRequest<bool>
{
    /// <summary>
    /// Create the create rewind mining pools command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind to.</param>
    public CreateRewindMiningPoolsCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}