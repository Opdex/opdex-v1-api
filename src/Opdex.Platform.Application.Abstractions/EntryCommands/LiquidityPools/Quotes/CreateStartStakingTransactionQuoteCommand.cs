using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    /// <summary>
    /// Quote a start staking transaction.
    /// </summary>
    public class CreateStartStakingTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a start staking transaction quote command.
        /// </summary>
        /// <param name="liquidityPool">The liquidity pool address.</param>
        /// <param name="wallet">The transaction sender's wallet address.</param>
        /// <param name="amount">The amount of tokens to start staking with.</param>
        /// <exception cref="ArgumentException">Invalid amount as a decimal string.</exception>
        public CreateStartStakingTransactionQuoteCommand(Address liquidityPool, Address wallet, FixedDecimal amount) : base(wallet)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentException("Liquidity pool must be provided.", nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
            Amount = amount;
        }

        public Address LiquidityPool { get; }
        public FixedDecimal Amount { get; }
    }
}
