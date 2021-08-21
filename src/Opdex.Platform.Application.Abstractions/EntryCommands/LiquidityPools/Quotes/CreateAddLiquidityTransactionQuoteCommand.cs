using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
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
        public CreateAddLiquidityTransactionQuoteCommand(Address liquidityPool, Address wallet, string amountCrs, string amountSrc,
                                                         string amountCrsMin, string amountSrcMin, Address recipient, DateTime? deadline) : base(wallet)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentException("Liquidity pool must be provided.", nameof(liquidityPool));
            }

            if (!amountCrs.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount CRS must be formatted as a decimal number.", nameof(amountCrs));
            }

            if (!amountSrc.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount SRC must be formatted as a decimal number.", nameof(amountSrc));
            }

            if (!amountCrsMin.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount CRS minimum must be formatted as a decimal number.", nameof(amountCrsMin));
            }

            if (!amountSrcMin.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount SRC minimum must be formatted as a decimal number.", nameof(amountSrcMin));
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentException("Recipient must be provided.", nameof(recipient));
            }

            if (deadline.HasValue && deadline < DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(deadline), "Deadline must be in the future.");
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
        public string AmountCrs { get; }
        public string AmountSrc { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public Address Recipient { get; }
        public DateTime? Deadline { get; }
    }
}
