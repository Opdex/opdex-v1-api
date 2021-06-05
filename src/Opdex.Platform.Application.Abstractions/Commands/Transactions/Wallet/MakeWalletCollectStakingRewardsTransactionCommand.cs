using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCollectStakingRewardsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletCollectStakingRewardsTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string liquidityPool, bool liquidate) : base(walletName, walletAddress, walletPassword)
        {
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
            Liquidate = liquidate;
        }
        
        public string LiquidityPool { get; }
        public bool Liquidate { get; }
    }
}