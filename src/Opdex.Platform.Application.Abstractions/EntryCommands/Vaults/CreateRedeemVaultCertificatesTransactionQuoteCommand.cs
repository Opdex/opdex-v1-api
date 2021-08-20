using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
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
        public CreateRedeemVaultCertificatesTransactionQuoteCommand(Address vault, Address holder) : base(vault, holder)
        {
        }
    }
}
