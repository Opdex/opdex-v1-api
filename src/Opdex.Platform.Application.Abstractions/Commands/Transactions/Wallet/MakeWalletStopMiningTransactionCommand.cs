using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStopMiningTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStopMiningTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string miningPool, string amount) : base(walletName, walletAddress, walletPassword)
        {
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }
            
            MiningPool = miningPool;
            Amount = amount;
        }
        
        public string MiningPool { get; }
        public string Amount { get; }
    }
}