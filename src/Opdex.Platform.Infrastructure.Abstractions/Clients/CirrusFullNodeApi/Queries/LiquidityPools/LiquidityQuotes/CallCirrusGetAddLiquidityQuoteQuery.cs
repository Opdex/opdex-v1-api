using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes
{
    public class CallCirrusGetAddLiquidityQuoteQuery : IRequest<UInt256>
    {
        public CallCirrusGetAddLiquidityQuoteQuery(UInt256 amountA, UInt256 reserveA, UInt256 reserveB, Address market)
        {
            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (amountA == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountA), "Amount A must be greater than 0.");
            }

            AmountA = amountA;
            ReserveA = reserveA;
            ReserveB = reserveB;
            Market = market;
        }

        public UInt256 AmountA { get; }
        public UInt256 ReserveA { get; }
        public UInt256 ReserveB { get; }
        public Address Market { get; }
    }
}
