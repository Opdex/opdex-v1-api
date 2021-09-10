using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCollectStakingRewardsTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletCollectStakingRewardsTransactionCommand(Address walletAddress, Address liquidityPool, bool liquidate) : base(walletAddress)
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
