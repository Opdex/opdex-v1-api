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
            Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            Holder = holder != Address.Empty ? holder : throw new ArgumentNullException(nameof(holder), "Holder address must be set.");
            Amount = amount > FixedDecimal.Zero ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
        }

        public Address Vault { get; }
        public Address Holder { get; }
        public FixedDecimal Amount { get; }
    }
}
