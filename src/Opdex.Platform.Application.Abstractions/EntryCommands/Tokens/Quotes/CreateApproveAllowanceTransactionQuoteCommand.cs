using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes
{
    /// <summary>
    /// Create an allowance approval transaction quote.
    /// </summary>
    public class CreateApproveAllowanceTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a reward mining pools transaction quote.
        /// </summary>
        /// <param name="token">The token contract address to call.</param>
        /// <param name="wallet">The wallet address public key sending the transaction.</param>
        /// <param name="spender">The spender's address.</param>
        /// <param name="amount">The amount of tokens to set for the allowance.</param>
        public CreateApproveAllowanceTransactionQuoteCommand(Address token, Address wallet, Address spender, FixedDecimal amount) : base(wallet)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Token address must be provided.");
            }

            if (spender == Address.Empty)
            {
                throw new ArgumentNullException(nameof(spender), "Spender address must be provided.");
            }

            if (amount <= FixedDecimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than or equal to 0.");
            }

            Token = token;
            Spender = spender;
            Amount = amount;
        }

        public Address Token { get; }
        public Address Spender { get; }
        public FixedDecimal Amount { get; }
    }
}
