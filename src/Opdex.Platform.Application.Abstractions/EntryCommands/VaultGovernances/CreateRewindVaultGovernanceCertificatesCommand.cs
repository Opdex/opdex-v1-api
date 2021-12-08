using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;

/// <summary>
/// Create a create rewind vault governance certificates command to refresh stale vault certificates.
/// </summary>
public class CreateRewindVaultGovernanceCertificatesCommand : IRequest<bool>
{
    /// <summary>
    /// Constructor for create rewind vault governance certificates command.
    /// </summary>
    /// <param name="rewindHeight">The block height to rewind certificates to.</param>
    public CreateRewindVaultGovernanceCertificatesCommand(ulong rewindHeight)
    {
        if (rewindHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rewindHeight), "Rewind height must be greater than 0.");
        }

        RewindHeight = rewindHeight;
    }

    public ulong RewindHeight { get; }
}
