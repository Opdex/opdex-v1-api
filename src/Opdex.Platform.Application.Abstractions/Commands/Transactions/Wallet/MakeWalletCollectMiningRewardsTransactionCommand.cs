using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCollectMiningRewardsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletCollectMiningRewardsTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string miningPool) : base(walletName, walletAddress, walletPassword)
        {
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }
            
            MiningPool = miningPool;
        }
        
        public string MiningPool { get; }
    }
}