using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateStartStakingTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateStartStakingTransactionQuoteCommand(Address pool, Address wallet, string amount) : base(pool, wallet)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount must be a valid decimal number.", nameof(amount));
            }

            Amount = amount;
        }

        public string Amount { get; }
    }
}
