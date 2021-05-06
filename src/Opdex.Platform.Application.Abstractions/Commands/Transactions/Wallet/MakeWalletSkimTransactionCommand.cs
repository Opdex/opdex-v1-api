using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletSkimTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletSkimTransactionCommand(string walletName, string walletAddress, string walletPassword, string liquidityPool, string recipient)
            : base(walletName, walletAddress, walletPassword)
        {
            if (!liquidityPool.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }
            
            if (!recipient.HasValue())
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            LiquidityPool = liquidityPool;
            Recipient = recipient;
        }
        
        public string LiquidityPool { get; }
        public string Recipient { get; }
    }
}