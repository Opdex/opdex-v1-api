using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRemoveLiquidityTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletRemoveLiquidityTransactionCommand(string walletAddress,
            string token, string liquidity, string amountCrsMin, string amountSrcMin, string recipient, string market)
            : base(walletAddress)
        {
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!liquidity.IsNumeric())
            {
                throw new ArgumentException(nameof(liquidity));
            }

            if (!amountCrsMin.IsNumeric())
            {
                throw new ArgumentException(nameof(amountCrsMin));
            }

            if (!amountSrcMin.IsNumeric())
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

            Token = token;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Market = market;
        }

        public string Token { get; }
        public string Liquidity { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string Recipient { get; }
        public string Market { get; }
    }
}
