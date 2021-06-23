using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletAddLiquidityTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletAddLiquidityTransactionCommand(string walletAddress, string pool, string amountCrs, string amountSrc, decimal tolerance,
                                                          string recipient, string market) : base(walletAddress)
        {
            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (!amountCrs.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(amountCrs));
            }

            if (!amountSrc.IsValidDecimalNumber())
            {
                throw new ArgumentException(nameof(amountSrc));
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

            LiquidityPool = pool;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            Tolerance = tolerance;
            Recipient = recipient;
            Market = market;
        }

        public string LiquidityPool { get; }
        public string AmountCrs { get; }
        public string AmountSrc { get; }
        public decimal Tolerance { get; }
        public string Recipient { get; }
        public string Market { get; }
    }
}
