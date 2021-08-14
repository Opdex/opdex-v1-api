using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    public class CreateStartMiningTransactionQuoteCommand : IRequest<TransactionQuoteDto>
    {
        public CreateStartMiningTransactionQuoteCommand(string walletAddress, string amount, string miningPool)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount must be a valid decimal number.", nameof(amount));
            }

            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool), "Mining pool must be provided.");
            }

            if (!walletAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(walletAddress), "Wallet address must be provided.");
            }

            Amount = amount;
            MiningPool = miningPool;
            WalletAddress = walletAddress;
        }

        public string Amount { get; }
        public string MiningPool { get; }
        public string WalletAddress { get; }
    }
}
