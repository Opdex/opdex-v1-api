using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountInMultiHopQuoteQuery : IRequest<UInt256>
    {
        public CallCirrusGetAmountInMultiHopQuoteQuery(Address router, UInt256 tokenOutAmount, ulong tokenOutReserveCrs, UInt256 tokenOutReserveSrc,
            ulong tokenInReserveCrs, UInt256 tokenInReserveSrc)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            if (tokenOutAmount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenOutAmount), "Token out amount must be greater than 0.");
            }

            Router = router;
            TokenOutAmount = tokenOutAmount;
            TokenOutReserveCrs = tokenOutReserveCrs;
            TokenOutReserveSrc = tokenOutReserveSrc;
            TokenInReserveCrs = tokenInReserveCrs;
            TokenInReserveSrc = tokenInReserveSrc;
        }

        public Address Router { get; }
        public UInt256 TokenOutAmount { get; }
        public ulong TokenOutReserveCrs { get; }
        public UInt256 TokenOutReserveSrc { get; }
        public ulong TokenInReserveCrs { get; }
        public UInt256 TokenInReserveSrc { get; }
    }
}
