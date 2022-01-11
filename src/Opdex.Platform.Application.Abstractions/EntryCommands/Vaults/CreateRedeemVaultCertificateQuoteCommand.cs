using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Command to quote redeeming a vault certificate.
/// </summary>
public class CreateRedeemVaultCertificateQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote redeeming a vault certificate.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="walletAddress">Address of the sender.</param>
    public CreateRedeemVaultCertificateQuoteCommand(Address vault, Address walletAddress) : base(walletAddress)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        Vault = vault;
    }

    public Address Vault { get; }
}
