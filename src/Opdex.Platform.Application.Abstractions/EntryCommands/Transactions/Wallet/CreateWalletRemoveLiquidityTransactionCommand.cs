using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRemoveLiquidityTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRemoveLiquidityTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string liquidityPool, string liquidity, string amountCrsMin, string amountSrcMin, string recipient, string market)
            : base(walletName, walletAddress, walletPassword)
        {
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            if (!liquidity.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(liquidity));
            }
            
            if (!amountCrsMin.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(amountCrsMin));
            }
            
            if (!amountSrcMin.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(amountSrcMin));
            }
            
            if (!recipient.HasValue())
            {
                throw new ArgumentNullException(nameof(recipient));
            }
            
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }
            
            LiquidityPool = liquidityPool;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Market = market;
        }
        
        public string Liquidity { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string LiquidityPool { get; }
        public string Recipient { get; }
        public string Market { get; }
    }
}