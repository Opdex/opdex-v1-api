using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStartStakingTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStartStakingTransactionCommand(string walletAddress,
            string amount, string liquidityPool) : base(walletAddress)
        {
            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount));
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
