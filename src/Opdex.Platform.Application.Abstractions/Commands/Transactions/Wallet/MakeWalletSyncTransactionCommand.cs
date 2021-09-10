using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletSyncTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletSyncTransactionCommand(Address walletAddress, Address liquidityPool) : base(walletAddress)
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
