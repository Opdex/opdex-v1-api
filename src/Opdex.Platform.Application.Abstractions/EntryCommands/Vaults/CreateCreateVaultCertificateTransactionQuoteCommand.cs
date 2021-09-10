using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    /// <summary>
    /// Quote a transaction to create a vault certificate.
    /// </summary>
    public class CreateCreateVaultCertificateTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a create vault certificate quote command.
        /// </summary>
        /// <param name="vault">The address of the vault.</param>
        /// <param name="owner">The vault owner address.</param>
        /// <param name="holder">The certificate holder address.</param>
        /// <param name="amount">The amount to store in the certificate.</param>
        /// <exception cref="ArgumentException">Invalid amount exception</exception>
        public CreateCreateVaultCertificateTransactionQuoteCommand(Address vault, Address owner, Address holder, FixedDecimal amount) : base(owner)
        {
            Vault = vault != Address.Empty ? vault : throw new ArgumentException("Vault address must be set.", nameof(vault));
            Holder = holder != Address.Empty ? holder : throw new ArgumentException("Holder address must be set.", nameof(holder));
            Amount = amount;
        }

        public Address Vault { get; }
        public Address Holder { get; }
        public FixedDecimal Amount { get; }
    }
}
