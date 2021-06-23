using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletSwapTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletSwapTransactionCommand(string walletAddress, string tokenIn, string tokenOut, string tokenInAmount, string tokenOutAmount,
                                                  bool tokenInExactAmount, decimal tolerance, string recipient, string market) : base(walletAddress)
        {
            if (!tokenIn.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (!tokenOut.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOut));
            }

            if (!tokenInAmount.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(tokenInAmount));
            }

            if (!tokenOutAmount.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(tokenInAmount));
            }

            if (tolerance > .9999m || tolerance < .0001m)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }

            if (!recipient.HasValue())
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            TokenInExactAmount = tokenInExactAmount;
            Tolerance = tolerance;
            Recipient = recipient;
            Market = market;
        }

        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public bool TokenInExactAmount { get; }
        public decimal Tolerance { get; }
        public string Recipient { get; }
        public string Market { get; }
    }
}
