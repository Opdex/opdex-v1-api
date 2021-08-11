using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Quote
{
    public class CreateStartMiningTransactionQuoteCommand : IRequest<TransactionQuoteDto>
    {
        public CreateStartMiningTransactionQuoteCommand(string walletAddress, string amount, string miningPool)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount must only contain numeric digits.", nameof(amount));
            }

            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            if (!walletAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(walletAddress));
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
