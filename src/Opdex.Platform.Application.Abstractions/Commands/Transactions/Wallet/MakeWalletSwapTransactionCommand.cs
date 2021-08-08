using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletSwapTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletSwapTransactionCommand(string walletAddress, string tokenIn, string tokenOut,
            string tokenInAmount, string tokenOutAmount, bool tokenInExactAmount, decimal tolerance, string recipient, string router)
            : base(walletAddress)
        {
            if (!tokenIn.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (!tokenOut.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenOut));
            }

            if (!tokenInAmount.IsNumeric())
            {
                throw new ArgumentException("Token in amount must only contain numeric digits.", nameof(tokenInAmount));
            }

            if (!tokenOutAmount.IsNumeric())
            {
                throw new ArgumentException("Token out amount must only contain numeric digits.", nameof(tokenOutAmount));
            }

            if (tolerance > .9999m || tolerance < .0001m)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }

            if (!recipient.HasValue())
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            if (!router.HasValue())
            {
                throw new ArgumentNullException(nameof(router));
            }

            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            TokenInExactAmount = tokenInExactAmount;
            Tolerance = tolerance;
            Recipient = recipient;
            Router = router;
        }

        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public bool TokenInExactAmount { get; }
        public decimal Tolerance { get; }
        public string Recipient { get; }
        public string Router { get; }
    }
}
