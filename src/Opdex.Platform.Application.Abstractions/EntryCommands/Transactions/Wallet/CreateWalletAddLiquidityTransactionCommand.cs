using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletAddLiquidityTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletAddLiquidityTransactionCommand(Address walletAddress, Address pool, decimal amountCrs, FixedDecimal amountSrc,
                                                          decimal tolerance, Address recipient, Address market) : base(walletAddress)
        {
            if (pool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(pool));
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

            LiquidityPool = pool;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            Tolerance = tolerance;
            Recipient = recipient;
            Market = market;
        }

        public Address LiquidityPool { get; }
        public decimal AmountCrs { get; }
        public FixedDecimal AmountSrc { get; }
        public decimal Tolerance { get; }
        public Address Recipient { get; }
        public Address Market { get; }
    }
}
