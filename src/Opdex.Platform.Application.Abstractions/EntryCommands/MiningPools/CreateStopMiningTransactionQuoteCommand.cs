using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    public class CreateStopMiningTransactionQuoteCommand : BaseQuoteCommand
    {
        public CreateStopMiningTransactionQuoteCommand(Address miningPool, Address walletAddress, string amount)
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
