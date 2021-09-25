using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes
{
    /// <summary>
    /// Quote a transaction to collect fees from a pool in a standard market.
    /// </summary>
    public class CreateCollectStandardMarketFeesTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a command to quote collecting fees from a pool in a standard market.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="owner">The address of the market owner.</param>
        /// <param name="token">The address of the token which is held in the pool.</param>
        /// <param name="amount">Total fees to collect, as the decimal token amount.</param>
        public CreateCollectStandardMarketFeesTransactionQuoteCommand(Address market, Address owner, Address token, FixedDecimal amount) : base(owner)
        {
            Market = market != Address.Empty ? market : throw new ArgumentNullException(nameof(market), "Market address must be set.");
            Token = token != Address.Empty ? token : throw new ArgumentNullException(nameof(token), "Token address must be set.");
            Amount = amount > FixedDecimal.Zero ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        public Address Market { get; }
        public Address Token { get; }
        public FixedDecimal Amount { get; }
    }
}
