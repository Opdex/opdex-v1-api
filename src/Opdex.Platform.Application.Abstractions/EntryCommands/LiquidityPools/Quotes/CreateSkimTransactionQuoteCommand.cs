using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    /// <summary>
    /// Quote a skim transaction.
    /// </summary>
    public class CreateSkimTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a skim transaction quote command.
        /// </summary>
        /// <param name="liquidityPool">The liquidity pool address.</param>
        /// <param name="wallet">The transaction sender's wallet address.</param>
        /// <param name="recipient">The recipient of any tokens skimmed.</param>
        /// <exception cref="ArgumentException">Invalid recipient address.</exception>
        public CreateSkimTransactionQuoteCommand(Address liquidityPool, Address wallet, Address recipient) : base(wallet)
        {
            LiquidityPool = liquidityPool != Address.Empty ? liquidityPool : throw new ArgumentException("Liquidity pool must be provided.", nameof(liquidityPool));
            Recipient = recipient != Address.Empty ? recipient : throw new ArgumentException("Recipient must be provided.", nameof(recipient));
        }

        public Address Recipient { get; }
        public Address LiquidityPool { get; }
    }
}
