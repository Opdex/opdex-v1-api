using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;

/// <summary>
/// Create rewind vault proposals command to refresh proposal data based at a certain block height.
/// </summary>
public class CreateRewindVaultProposalsCommand : IRequest<bool>
{
    /// <summary>
    /// Constructor to initialize the create rewind vault proposals command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind to.</param>
    public CreateRewindVaultProposalsCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}
