using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets
{
    /// <summary>
    /// Quote a transaction to set ownership of a standard market.
    /// </summary>
    public class CreateSetStandardMarketOwnershipTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a command to quote setting a new owner for a standard market.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="currentOwner">The address of the current market owner.</param>
        /// <param name="newOwner">The address of the new market owner.</param>
        public CreateSetStandardMarketOwnershipTransactionQuoteCommand(Address market, Address currentOwner, Address newOwner) : base(currentOwner)
        {
            Market = market != Address.Empty ? market : throw new ArgumentNullException(nameof(market), "Market address must be set.");
            NewOwner = newOwner != Address.Empty ? newOwner : throw new ArgumentNullException(nameof(newOwner), "New owner address must be set.");
        }

        public Address Market { get; }
        public Address NewOwner { get; }
    }
}
