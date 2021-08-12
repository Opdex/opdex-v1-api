using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Quote
{
    public class MakeStartMiningTransactionQuoteCommand : IRequest<TransactionQuote>
    {
        public MakeStartMiningTransactionQuoteCommand(string walletAddress, string amount, string miningPool)
        {
            if (!amount.IsNumeric())
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
