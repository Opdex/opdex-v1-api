using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateStopStakingTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateStopStakingTransactionQuoteCommand(Address pool, Address wallet, string amount, bool liquidate) : base(pool, wallet)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount must be a valid decimal number.", nameof(amount));
            }

            Amount = amount;
            Liquidate = liquidate;
        }

        public string Amount { get; }
        public bool Liquidate { get; }
    }
}
