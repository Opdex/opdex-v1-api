using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    /// <summary>
    /// Quote a transaction to revoke vault certificates of a certificate holder.
    /// </summary>
    public class CreateRevokeVaultCertificatesTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a revoke vault certificates quote command.
        /// </summary>
        /// <param name="vault">The address of the vault.</param>
        /// <param name="owner">The vault owner address.</param>
        /// <param name="holder">The address of the certificate holder.</param>
        public CreateRevokeVaultCertificatesTransactionQuoteCommand(Address vault, Address owner, Address holder) : base(owner)
        {
            Vault = vault != Address.Empty ? vault : throw new ArgumentException("Vault address must be provided.", nameof(vault));
            Holder = holder != Address.Empty ? holder : throw new ArgumentException("Certificate holder address must be provided.", nameof(holder));
        }

        public Address Vault { get; }
        public Address Holder { get; }
    }
}
