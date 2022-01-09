using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Create a new vault record if one does not already exist.
/// </summary>
public class CreateVaultGovernanceCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to initialize the create vault command.
    /// </summary>
    /// <param name="vault">The address of the vault smart contract.</param>
    /// <param name="tokenId">The tokenId of the locked vault token.</param>
    /// <param name="blockHeight">The block height to retrieve values at.</param>
    public CreateVaultGovernanceCommand(Address vault, ulong tokenId, ulong blockHeight)
    {
        if (vault == Address.Empty)
        {
            throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        }

        if (tokenId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenId), "Token Id must be greater than zero.");
        }

        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        Vault = vault;
        TokenId = tokenId;
        BlockHeight = blockHeight;
    }

    public Address Vault { get; }
    public ulong TokenId { get; }
    public ulong BlockHeight { get; }
}
