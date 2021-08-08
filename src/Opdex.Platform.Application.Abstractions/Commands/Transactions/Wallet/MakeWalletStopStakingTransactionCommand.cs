using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStopStakingTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStopStakingTransactionCommand(string walletAddress,
            string liquidityPool, string amount, bool liquidate) : base(walletAddress)
        {
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            LiquidityPool = liquidityPool;
            Liquidate = liquidate;
            Amount = amount;
        }

        public string LiquidityPool { get; }
        public bool Liquidate { get; }
        public string Amount { get; }
    }
}
