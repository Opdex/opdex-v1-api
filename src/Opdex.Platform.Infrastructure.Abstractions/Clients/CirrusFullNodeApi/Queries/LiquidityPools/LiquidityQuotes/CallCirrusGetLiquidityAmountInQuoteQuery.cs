using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes
{
    public class CallCirrusGetLiquidityAmountInQuoteQuery : IRequest<UInt256>
    {
        public CallCirrusGetLiquidityAmountInQuoteQuery(UInt256 amountA, UInt256 reserveA, UInt256 reserveB, Address router)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            if (amountA == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountA), "Amount A must be greater than 0.");
            }

            AmountA = amountA;
            ReserveA = reserveA;
            ReserveB = reserveB;
            Router = router;
        }

        public UInt256 AmountA { get; }
        public UInt256 ReserveA { get; }
        public UInt256 ReserveB { get; }
        public Address Router { get; }
    }
}
