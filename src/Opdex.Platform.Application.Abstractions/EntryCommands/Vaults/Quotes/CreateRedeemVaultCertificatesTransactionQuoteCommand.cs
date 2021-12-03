using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;

/// <summary>
/// Quote a transaction to for a certificate holder to redeem vault certificates.
/// </summary>
public class CreateRedeemVaultCertificatesTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a redeem vault certificates quote command.
    /// </summary>
    /// <param name="vault">The address of the vault.</param>
    /// <param name="holder">The address of the current owner.</param>
    public CreateRedeemVaultCertificatesTransactionQuoteCommand(Address vault, Address holder) : base(holder)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
    }

    public Address Vault { get; }
}