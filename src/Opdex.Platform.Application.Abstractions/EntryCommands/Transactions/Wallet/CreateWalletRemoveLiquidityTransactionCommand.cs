using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRemoveLiquidityTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRemoveLiquidityTransactionCommand(Address walletAddress, Address liquidityPool, FixedDecimal liquidity, FixedDecimal amountCrsMin,
                                                             FixedDecimal amountSrcMin, Address recipient, Address router) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            if (router == Address.Empty)
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

        public FixedDecimal Liquidity { get; }
        public FixedDecimal AmountCrsMin { get; }
        public FixedDecimal AmountSrcMin { get; }
        public Address LiquidityPool { get; }
        public Address Recipient { get; }
        public Address Router { get; }
    }
}
