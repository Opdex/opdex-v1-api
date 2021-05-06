using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletAddLiquidityTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletAddLiquidityTransactionCommand(string walletName, string walletAddress, string walletPassword, string token, 
            string amountCrs, string amountSrc, string amountCrsMin, string amountSrcMin, string recipient, string market) 
            : base(walletName, walletAddress, walletPassword)
        {
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }

            // Should passthrough with a decimal
            if (!amountCrs.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(amountCrs));
            }
            
            if (!amountSrc.IsNumeric())
            {
                throw new ArgumentException(nameof(amountSrc));
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
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            Recipient = recipient;
            Market = market;
        }
        
        public string Token { get; }
        public string AmountCrs { get; }
        public string AmountSrc { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string Recipient { get; }
        public string Market { get; }
    }
}