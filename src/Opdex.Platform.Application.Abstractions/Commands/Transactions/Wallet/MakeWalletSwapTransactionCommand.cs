using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletSwapTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletSwapTransactionCommand(Address walletAddress, Address tokenIn, Address tokenOut,
            UInt256 tokenInAmount, UInt256 tokenOutAmount, bool tokenInExactAmount, decimal tolerance, Address recipient, Address router)
            : base(walletAddress)
        {
            if (tokenIn == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (tokenOut == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenOut));
            }

            if (tolerance > .9999m || tolerance < .0001m)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            if (router == Address.Empty)
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

        public Address TokenIn { get; }
        public Address TokenOut { get; }
        public UInt256 TokenInAmount { get; }
        public UInt256 TokenOutAmount { get; }
        public bool TokenInExactAmount { get; }
        public decimal Tolerance { get; }
        public Address Recipient { get; }
        public Address Router { get; }
    }
}
