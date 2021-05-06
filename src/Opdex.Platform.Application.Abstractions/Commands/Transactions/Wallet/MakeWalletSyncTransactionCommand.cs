using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletSyncTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletSyncTransactionCommand(string walletName, string walletAddress, string walletPassword, string liquidityPool)
            : base(walletName, walletAddress, walletPassword)
        {
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
        }
        
        public string LiquidityPool { get; }
    }
}