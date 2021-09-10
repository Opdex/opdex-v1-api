using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountOutMultiHopQuoteQuery : IRequest<UInt256>
    {
        public CallCirrusGetAmountOutMultiHopQuoteQuery(Address router, UInt256 tokenInAmount, ulong tokenInReserveCrs, UInt256 tokenInReserveSrc,
            ulong tokenOutReserveCrs, UInt256 tokenOutReserveSrc)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            Router = router;
            TokenInAmount = tokenInAmount;
            TokenInReserveCrs = tokenInReserveCrs;
            TokenInReserveSrc = tokenInReserveSrc;
            TokenOutReserveCrs = tokenOutReserveCrs;
            TokenOutReserveSrc = tokenOutReserveSrc;
        }

        public Address Router { get; }
        public UInt256 TokenInAmount { get; }
        public ulong TokenOutReserveCrs { get; }
        public UInt256 TokenOutReserveSrc { get; }
        public ulong TokenInReserveCrs { get; }
        public UInt256 TokenInReserveSrc { get; }
    }
}
