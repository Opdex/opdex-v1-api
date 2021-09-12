using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountInStandardQuoteQuery : IRequest<UInt256>
    {
        public CallCirrusGetAmountInStandardQuoteQuery(Address router, UInt256 amountOut, UInt256 reserveIn, UInt256 reserveOut)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            if (amountOut == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountOut), "Amount out must be greater than 0.");
            }

            Router = router;
            AmountOut = amountOut;
            ReserveIn = reserveIn;
            ReserveOut = reserveOut;
        }

        public Address Router { get; }
        public UInt256 AmountOut { get; }
        public UInt256 ReserveIn { get; }
        public UInt256 ReserveOut { get; }
    }
}
