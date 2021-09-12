using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletAddLiquidityTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletAddLiquidityTransactionCommand(Address walletAddress, Address token, FixedDecimal amountCrs, UInt256 amountSrc,
                                                        ulong amountCrsMin, UInt256 amountSrcMin, Address recipient, Address router) : base(walletAddress)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            Token = token;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Router = router;
        }

        public Address Token { get; }
        public FixedDecimal AmountCrs { get; }
        public UInt256 AmountSrc { get; }
        public ulong AmountCrsMin { get; }
        public UInt256 AmountSrcMin { get; }
        public Address Recipient { get; }
        public Address Router { get; }
    }
}
