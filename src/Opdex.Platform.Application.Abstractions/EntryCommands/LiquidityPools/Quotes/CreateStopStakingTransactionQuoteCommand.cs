using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    /// <summary>
    /// Quote a stop staking transaction.
    /// </summary>
    public class CreateStopStakingTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a stop staking transaction quote command.
        /// </summary>
        /// <param name="liquidityPool">The liquidity pool address.</param>
        /// <param name="wallet">The transaction sender's wallet address.</param>
        /// <param name="amount">The amount of tokens to stop staking with.</param>
        /// <param name="liquidate">Flag indicating if rewarded OLPT tokens should be liquidated into the pool's underlying reserve tokens.</param>
        /// <exception cref="ArgumentException">Invalid amount as a decimal string.</exception>
        public CreateStopStakingTransactionQuoteCommand(Address liquidityPool, Address wallet, FixedDecimal amount, bool liquidate) : base(wallet)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentException("Liquidity pool must be provided.", nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
            Amount = amount;
            Liquidate = liquidate;
        }

        public Address LiquidityPool { get; }
        public FixedDecimal Amount { get; }
        public bool Liquidate { get; }
    }
}
