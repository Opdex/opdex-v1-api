using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Create rewind vault proposal pledges command to refresh proposal pledge data based at a certain block height.
/// </summary>
public class CreateRewindVaultProposalPledgesCommand : IRequest<bool>
{
    /// <summary>
    /// Constructor to initialize the create rewind vault proposal pledges command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind to.</param>
    public CreateRewindVaultProposalPledgesCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}
