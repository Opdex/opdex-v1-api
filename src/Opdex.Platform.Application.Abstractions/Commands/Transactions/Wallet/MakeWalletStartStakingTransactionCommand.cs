using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStartStakingTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStartStakingTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string amount, string liquidityPool) : base(walletName, walletAddress, walletPassword)
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