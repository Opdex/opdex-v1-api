using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCollectStakingRewardsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletCollectStakingRewardsTransactionCommand(Address walletAddress, Address liquidityPool, bool liquidate) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
            Liquidate = liquidate;
        }

        public Address LiquidityPool { get; }
        public bool Liquidate { get; }
    }
}
