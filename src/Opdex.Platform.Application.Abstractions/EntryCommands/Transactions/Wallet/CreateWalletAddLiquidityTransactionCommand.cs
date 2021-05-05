using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletAddLiquidityTransactionCommand : IRequest<string>
    {
        public CreateWalletAddLiquidityTransactionCommand(string pool, string amountCrs, string amountSrc, 
            decimal tolerance, string to, string market)
        {
            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (!amountCrs.HasValue() || !amountCrs.Contains('.'))
            {
                throw new ArgumentException(nameof(amountCrs));
            }
            
            if (!amountSrc.HasValue() || !amountSrc.Contains('.'))
            {
                throw new ArgumentException(nameof(amountSrc));
            }
            
            if (tolerance > 1 || tolerance < .01m)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }

            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }
            
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }
            
            Pool = pool;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            Tolerance = tolerance;
            To = to;
            Market = market;
        }
        
        public string Pool { get; }
        public string AmountCrs { get; }
        public string AmountSrc { get; }
        public decimal Tolerance { get; }
        public string To { get; }
        public string Market { get; }
    }
}