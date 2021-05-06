using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStartMiningTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStartMiningTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string amount, string miningPool) : base(walletName, walletAddress, walletPassword)
        {
            if (!amount.IsNumeric())
            {
                throw new ArgumentException(nameof(amount));
            }
            
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            Amount = amount;
            MiningPool = miningPool;
        }
        
        public string Amount { get; }
        public string MiningPool { get; }
    }
}