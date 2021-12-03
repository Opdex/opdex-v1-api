using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;

/// <summary>
/// Creates a rewind deployers command to rewind all applicable deployers' properties back to a specific block height.
/// </summary>
public class CreateRewindDeployersCommand : IRequest<bool>
{
    /// <summary>
    /// Creates a rewind deployers command.
    /// </summary>
    /// <param name="rewindHeight">The block number height to rewind to.</param>
    public CreateRewindDeployersCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}