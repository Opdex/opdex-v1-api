using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStopStakingTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStopStakingTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string recipient, string liquidityPool, bool liquidate) : base(walletName, walletAddress, walletPassword)
        {
            if (!recipient.HasValue())
            {
                throw new ArgumentNullException(nameof(recipient));
            }
            
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            Recipient = recipient;
            LiquidityPool = liquidityPool;
            Liquidate = liquidate;
        }
        
        public string Recipient { get; }
        public string LiquidityPool { get; }
        public bool Liquidate { get; }
    }
}