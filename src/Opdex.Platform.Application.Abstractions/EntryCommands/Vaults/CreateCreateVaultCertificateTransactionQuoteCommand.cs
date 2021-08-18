using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
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
        /// Creates a create vault certificate quote command
        /// </summary>
        /// <param name="vault">The address of the vault.</param>
        /// <param name="wallet">The transaction sender's wallet address.</param>
        /// <param name="holder">The certificate holder address.</param>
        /// <param name="amount">The amount to store in the certificate.</param>
        /// <exception cref="ArgumentException">Invalid amount exception</exception>
        public CreateCreateVaultCertificateTransactionQuoteCommand(Address vault, Address wallet, Address holder, string amount) : base(vault, wallet)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount must be a valid decimal number.", nameof(amount));
            }

            Holder = holder != Address.Empty ? holder : throw new ArgumentNullException(nameof(holder), "Holder address must be set.");
            Amount = amount;
        }

        public Address Holder { get; }
        public string Amount { get; }
    }
}
