using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes
{
    /// <summary>
    /// Quote a swap transaction.
    /// </summary>
    public class CreateSwapTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Constructor to create a swap transaction quote comment.
        /// </summary>
        /// <param name="tokenIn">The input token address.</param>
        /// <param name="wallet">The wallet address initiating the transaction quote.</param>
        /// <param name="tokenOut">The output token address.</param>
        /// <param name="tokenInAmount">The hopeful amount of tokens to be input, spent by the wallet.</param>
        /// <param name="tokenOutAmount">The hopeful amount of tokens to be output, received by the wallet.</param>
        /// <param name="tokenInMaximumAmount">The maximum amount of tokens to be input, spent by the wallet.</param>
        /// <param name="tokenOutMinimumAmount">The minimum amount of tokens to be output, received by the wallet.</param>
        /// <param name="tokenInExactAmount">Flag indicating if the input tokens is the exact amount or if the output token amount is expected to be exact.</param>
        /// <param name="recipient">The recipient's wallet address of the received tokens.</param>
        /// <param name="market">The market address of the token in and it's liquidity pool.</param>
        /// <param name="deadline">The block deadline that the transaction is valid before.</param>
        public CreateSwapTransactionQuoteCommand(Address tokenIn, Address wallet, Address tokenOut, FixedDecimal tokenInAmount,
                                                 FixedDecimal tokenOutAmount, FixedDecimal tokenInMaximumAmount, FixedDecimal tokenOutMinimumAmount,
                                                 bool tokenInExactAmount, Address recipient, Address market, ulong deadline) : base(wallet)
        {
            TokenIn = tokenIn != Address.Empty ? tokenIn : throw new ArgumentNullException(nameof(tokenIn), "Token in address must be provided.");
            TokenOut = tokenOut != Address.Empty ? tokenOut : throw new ArgumentNullException(nameof(tokenOut), "Token out address must be provided.");
            TokenInAmount = tokenInAmount != FixedDecimal.Zero ? tokenInAmount : throw new ArgumentOutOfRangeException(nameof(tokenInAmount), "Token in amount must be greater than zero.");
            TokenOutAmount = tokenOutAmount != FixedDecimal.Zero ? tokenOutAmount : throw new ArgumentOutOfRangeException(nameof(tokenOutAmount), "Token out amount must be greater than zero.");
            TokenInMaximumAmount = tokenInMaximumAmount != FixedDecimal.Zero ? tokenInMaximumAmount : throw new ArgumentOutOfRangeException(nameof(tokenInMaximumAmount), "Token in maximum amount must be greater than zero.");
            TokenOutMinimumAmount = tokenOutMinimumAmount != FixedDecimal.Zero ? tokenOutMinimumAmount : throw new ArgumentOutOfRangeException(nameof(tokenOutMinimumAmount), "Token out minimum amount must be greater than zero.");
            Recipient = recipient != Address.Empty ? recipient : throw new ArgumentNullException(nameof(recipient), "Recipient address must be provided.");
            Market = market != Address.Empty ? market : throw new ArgumentNullException(nameof(market), "Market address must be provided.");
            TokenInExactAmount = tokenInExactAmount;
            Deadline = deadline;
        }

        public Address TokenIn { get; }
        public Address TokenOut { get; }
        public FixedDecimal TokenInAmount { get; }
        public FixedDecimal TokenOutAmount { get; }
        public FixedDecimal TokenInMaximumAmount { get; }
        public FixedDecimal TokenOutMinimumAmount { get; }
        public bool TokenInExactAmount { get; }
        public Address Recipient { get; }
        public Address Market { get; }
        public ulong Deadline { get; }
    }
}
