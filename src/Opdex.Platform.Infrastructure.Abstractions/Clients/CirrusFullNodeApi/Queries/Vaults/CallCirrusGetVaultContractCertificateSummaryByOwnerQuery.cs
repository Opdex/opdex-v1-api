using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;

/// <summary>
/// Get the summary of a specific vault certificate directly from the smart contract at a specific block in time.
/// </summary>
public class CallCirrusGetVaultContractCertificateSummaryByOwnerQuery : IRequest<VaultContractCertificateSummary>
{
    /// <summary>
    /// Constructor to initialize a call cirrus get vault contract certificate summary by owner query.
    /// </summary>
    /// <param name="vault">The address of the vault to check.</param>
    /// <param name="owner">The certificate owner's address.</param>
    /// <param name="blockHeight">The block height to query the vault at.</param>
    public CallCirrusGetVaultContractCertificateSummaryByOwnerQuery(Address vault, Address owner, ulong blockHeight)
    {
        if (vault == Address.Empty)
        {
            throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        }

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
        }

        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        Vault = vault;
        Owner = owner;
        BlockHeight = blockHeight;
    }

    public Address Vault { get; }
    public Address Owner { get; }
    public ulong BlockHeight { get; }
}
