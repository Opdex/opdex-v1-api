using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    /// <summary>
    /// Quote a transaction to claim pending vault ownership.
    /// </summary>
    public class CreateClaimPendingVaultOwnershipTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a claim pending vault ownership quote command.
        /// </summary>
        /// <param name="vault">The address of the vault.</param>
        /// <param name="pendingOwner">The address of the pending owner.</param>
        public CreateClaimPendingVaultOwnershipTransactionQuoteCommand(Address vault, Address pendingOwner) : base(vault, pendingOwner)
        {
        }
    }
}
