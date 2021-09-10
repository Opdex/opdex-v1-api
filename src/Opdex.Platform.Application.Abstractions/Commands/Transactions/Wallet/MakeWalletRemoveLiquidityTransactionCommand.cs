using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRemoveLiquidityTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletRemoveLiquidityTransactionCommand(Address walletAddress, Address token, UInt256 liquidity, ulong amountCrsMin,
                                                           UInt256 amountSrcMin, Address recipient, Address market) : base(walletAddress)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market));
            }

            Token = token;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Market = market;
        }

        public Address Token { get; }
        public UInt256 Liquidity { get; }
        public ulong AmountCrsMin { get; }
        public UInt256 AmountSrcMin { get; }
        public Address Recipient { get; }
        public Address Market { get; }
    }
}
