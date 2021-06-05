using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletStopStakingTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletStopStakingTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string liquidityPool, string amount, bool liquidate) : base(walletName, walletAddress, walletPassword)
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
            Liquidate = liquidate;
            Amount = amount;
        }
        
        public string LiquidityPool { get; }
        public bool Liquidate { get; }
        public string Amount { get; }
    }
}