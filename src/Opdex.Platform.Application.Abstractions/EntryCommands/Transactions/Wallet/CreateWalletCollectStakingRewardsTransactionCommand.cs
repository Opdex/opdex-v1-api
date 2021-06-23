using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCollectStakingRewardsTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletCollectStakingRewardsTransactionCommand(string walletAddress, string liquidityPool, bool liquidate) : base(walletAddress)
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
