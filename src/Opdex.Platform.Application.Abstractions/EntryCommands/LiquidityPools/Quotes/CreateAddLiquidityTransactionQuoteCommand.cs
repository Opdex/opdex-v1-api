using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateAddLiquidityTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateAddLiquidityTransactionQuoteCommand(Address liquidityPool, Address wallet, string amountCrs, string amountSrc,
                                                         string amountCrsMin, string amountSrcMin, Address recipient, DateTime? deadline) : base(wallet)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool must be provided.");
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
                throw new ArgumentNullException(nameof(recipient), "Recipient must be provided.");
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
