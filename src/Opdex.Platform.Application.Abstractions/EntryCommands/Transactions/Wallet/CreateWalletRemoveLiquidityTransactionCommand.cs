using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRemoveLiquidityTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRemoveLiquidityTransactionCommand(string walletAddress, string liquidityPool, string liquidity, string amountCrsMin,
                                                             string amountSrcMin, string recipient, string router) : base(walletAddress)
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

            if (!router.HasValue())
            {
                throw new ArgumentNullException(nameof(router));
            }

            LiquidityPool = liquidityPool;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Router = router;
        }

        public string Liquidity { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string LiquidityPool { get; }
        public string Recipient { get; }
        public string Router { get; }
    }
}
