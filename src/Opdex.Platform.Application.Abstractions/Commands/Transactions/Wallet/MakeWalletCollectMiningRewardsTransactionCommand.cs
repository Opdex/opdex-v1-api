using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCollectMiningRewardsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletCollectMiningRewardsTransactionCommand(Address walletAddress, Address miningPool) : base(walletAddress)
        {
            if (miningPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            MiningPool = miningPool;
        }

        public Address MiningPool { get; }
    }
}
