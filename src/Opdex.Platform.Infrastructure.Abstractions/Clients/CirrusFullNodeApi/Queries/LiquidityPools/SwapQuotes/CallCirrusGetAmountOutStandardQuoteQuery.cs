using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountOutStandardQuoteQuery : IRequest<UInt256>
    {
        public CallCirrusGetAmountOutStandardQuoteQuery(Address router, UInt256 amountIn, UInt256 reserveIn, UInt256 reserveOut)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            Router = router;
            AmountIn = amountIn;
            ReserveIn = reserveIn;
            ReserveOut = reserveOut;
        }

        public Address Router { get; }
        public UInt256 AmountIn { get; }
        public UInt256 ReserveIn { get; }
        public UInt256 ReserveOut { get; }
    }
}
