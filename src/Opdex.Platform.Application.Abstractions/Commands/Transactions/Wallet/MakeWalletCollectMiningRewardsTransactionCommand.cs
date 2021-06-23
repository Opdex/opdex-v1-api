using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCollectMiningRewardsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletCollectMiningRewardsTransactionCommand(string walletAddress,
            string miningPool) : base(walletAddress)
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
