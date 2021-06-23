using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletStartMiningTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletStartMiningTransactionCommand(string walletAddress, string amount, string liquidityPool) : base(walletAddress)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(amount));
            }

            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            Amount = amount;
            LiquidityPool = liquidityPool;
        }

        public string Amount { get; }
        public string LiquidityPool { get; }
    }
}
