using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    public class CreateStartMiningTransactionQuoteCommand : BaseQuoteCommand
    {
        public CreateStartMiningTransactionQuoteCommand(Address miningPool, Address walletAddress, string amount)
            : base(miningPool, walletAddress)
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
