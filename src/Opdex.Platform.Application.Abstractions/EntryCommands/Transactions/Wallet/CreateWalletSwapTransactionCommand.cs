using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletSwapTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletSwapTransactionCommand(Address walletAddress, Address tokenIn, Address tokenOut, FixedDecimal tokenInAmount, FixedDecimal tokenOutAmount,
                                                  bool tokenInExactAmount, decimal tolerance, Address recipient, Address market) : base(walletAddress)
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

            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (tokenInAmount <= FixedDecimal.Zero && tokenOutAmount <= FixedDecimal.Zero)
            {
                throw new ArgumentException("Token in amount or token out amount need to have a value.");
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

        public Address TokenIn { get; }
        public Address TokenOut { get; }
        public FixedDecimal TokenInAmount { get; }
        public FixedDecimal TokenOutAmount { get; }
        public bool TokenInExactAmount { get; }
        public decimal Tolerance { get; }
        public Address Recipient { get; }
        public Address Market { get; }
    }
}
