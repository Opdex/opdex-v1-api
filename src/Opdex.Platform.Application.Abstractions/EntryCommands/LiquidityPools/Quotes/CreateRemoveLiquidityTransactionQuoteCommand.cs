using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateRemoveLiquidityTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateRemoveLiquidityTransactionQuoteCommand(Address liquidityPool, Address wallet, string amountLpt, string amountCrsMin,
                                                            string amountSrcMin, Address recipient, DateTime? deadline) : base(wallet)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool must be provided.");
            }

            if (!amountLpt.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount LPT burned must be formatted as a decimal number.", nameof(amountLpt));
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
            AmountLpt = amountLpt;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Deadline = deadline;
        }

        public Address LiquidityPool { get; }
        public string AmountLpt { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public Address Recipient { get; }
        public DateTime? Deadline { get; }
    }
}
