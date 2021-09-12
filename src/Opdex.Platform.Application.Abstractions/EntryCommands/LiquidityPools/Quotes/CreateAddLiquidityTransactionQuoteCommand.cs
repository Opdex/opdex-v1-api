using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    /// <summary>
    /// Quote an add liquidity transaction.
    /// </summary>
    public class CreateAddLiquidityTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates an add liquidity transaction quote command.
        /// </summary>
        /// <param name="liquidityPool">The address of the liquidity pool.</param>
        /// <param name="wallet">The address of the wallet sending the transaction.</param>
        /// <param name="amountCrs">The preferred amount of CRS tokens to add.</param>
        /// <param name="amountSrc">The preferred amount of SRC tokens to add.</param>
        /// <param name="amountCrsMin">The minimum amount of CRS tokens to add.</param>
        /// <param name="amountSrcMin">The minimum amount of STC tokens to add.</param>
        /// <param name="recipient">The recipient of the returned liquidity pool tokens.</param>
        /// <param name="deadline">The block deadline that the transaction is valid before.</param>
        /// <exception cref="ArgumentException">Invalid liquidity pool, amounts, or recipient command parameters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid deadline</exception>
        public CreateAddLiquidityTransactionQuoteCommand(Address liquidityPool, Address wallet, FixedDecimal amountCrs, FixedDecimal amountSrc,
                                                         FixedDecimal amountCrsMin, FixedDecimal amountSrcMin, Address recipient, ulong deadline) : base(wallet)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool must be provided.");
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentNullException(nameof(recipient), "Recipient must be provided.");
            }

            if (amountCrs <= FixedDecimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs), "Amount CRS must be greater than 0.");
            }

            if (amountSrc <= FixedDecimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amountSrc), "Amount SRC must be greater than 0.");
            }

            if (amountCrsMin <= FixedDecimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrsMin), "Amount CRS minimum must be greater than 0.");
            }

            if (amountSrcMin <= FixedDecimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amountSrcMin), "Amount SRC minimum must be greater than 0.");
            }

            LiquidityPool = liquidityPool;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Deadline = deadline;
        }

        public Address LiquidityPool { get; }
        public FixedDecimal AmountCrs { get; }
        public FixedDecimal AmountSrc { get; }
        public FixedDecimal AmountCrsMin { get; }
        public FixedDecimal AmountSrcMin { get; }
        public Address Recipient { get; }
        public ulong Deadline { get; }
    }
}
