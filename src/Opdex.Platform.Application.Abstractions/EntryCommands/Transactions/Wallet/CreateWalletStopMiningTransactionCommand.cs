using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletStopMiningTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletStopMiningTransactionCommand(string walletAddress, string liquidityPool, string amount) : base(walletAddress)
        {
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            LiquidityPool = liquidityPool;
            Amount = amount;
        }

        public string LiquidityPool { get; }
        public string Amount { get; }
    }
}
