using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletSyncTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletSyncTransactionCommand(string walletAddress, string liquidityPool) : base(walletAddress)
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
