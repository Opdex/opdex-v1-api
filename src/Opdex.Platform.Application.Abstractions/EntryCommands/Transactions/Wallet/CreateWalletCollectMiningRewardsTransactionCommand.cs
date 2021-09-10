using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCollectMiningRewardsTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletCollectMiningRewardsTransactionCommand(Address walletAddress, Address liquidityPool) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
        }

        public Address LiquidityPool { get; }
    }
}
