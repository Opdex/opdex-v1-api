using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets;

/// <summary>
/// Create rewind markets command to refresh market data based at a certain block height.
/// </summary>
public class CreateRewindMarketsCommand : IRequest<bool>
{
    /// <summary>
    /// Create the create rewind markets command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind to.</param>
    public CreateRewindMarketsCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}