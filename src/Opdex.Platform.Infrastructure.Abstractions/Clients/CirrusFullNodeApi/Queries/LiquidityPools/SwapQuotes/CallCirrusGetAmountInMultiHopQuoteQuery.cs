using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    /// <summary>
    /// Retrieves the token input amount that would be required for a SRC-SRC swap transaction, given an exact token output amount.
    /// </summary>
    public class CallCirrusGetAmountInMultiHopQuoteQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a query to retrieve the token input amount that would be required for a SRC-SRC swap transaction, given an exact token output amount.
        /// </summary>
        /// <param name="router">The address of the router contract.</param>
        /// <param name="tokenOutAmount">The token output amount.</param>
        /// <param name="tokenOutReserveCrs">The CRS reserve amount of the output token liquidity pool.</param>
        /// <param name="tokenOutReserveSrc">The SRC reserve amount of the output token liquidity pool.</param>
        /// <param name="tokenInReserveCrs">The CRS reserve amount of the input token liquidity pool.</param>
        /// <param name="tokenInReserveSrc">The SRC reserve amount of the input token liquidity pool.</param>
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
