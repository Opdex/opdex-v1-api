using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes
{
    /// <summary>
    /// Quote a transaction to claim ownership of a standard market.
    /// </summary>
    public class CreateClaimStandardMarketOwnershipTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a command to quote claiming pending ownership of a standard market.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="pendingOwner">The address of the pending owner.</param>
        public CreateClaimStandardMarketOwnershipTransactionQuoteCommand(Address market, Address pendingOwner) : base(pendingOwner)
        {
            Market = market != Address.Empty ? market : throw new ArgumentNullException(nameof(market), "Market address must be set.");
        }

        public Address Market { get; }
    }
}
