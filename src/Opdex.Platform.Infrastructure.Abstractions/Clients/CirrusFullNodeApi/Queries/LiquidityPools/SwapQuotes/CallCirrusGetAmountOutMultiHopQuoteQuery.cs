using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    /// <summary>
    /// Retrieves the token output amount that would be required for a SRC-SRC swap transaction, given an exact token input amount.
    /// </summary>
    public class CallCirrusGetAmountOutMultiHopQuoteQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a query to retrieve the token output amount that would be required for a SRC-SRC swap transaction, given an exact token input amount.
        /// </summary>
        /// <param name="router">The address of the router contract.</param>
        /// <param name="tokenInAmount">The token input amount.</param>
        /// <param name="tokenInReserveCrs">The CRS reserve amount of the input token liquidity pool.</param>
        /// <param name="tokenInReserveSrc">The SRC reserve amount of the input token liquidity pool.</param>
        /// <param name="tokenOutReserveCrs">The CRS reserve amount of the output token liquidity pool.</param>
        /// <param name="tokenOutReserveSrc">The SRC reserve amount of the output token liquidity pool.</param>
        public CallCirrusGetAmountOutMultiHopQuoteQuery(Address router, UInt256 tokenInAmount, ulong tokenInReserveCrs, UInt256 tokenInReserveSrc,
            ulong tokenOutReserveCrs, UInt256 tokenOutReserveSrc)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router), "The router address must be set.");
            }

            if (tokenInAmount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenInAmount), "Token in amount must be greater than 0.");
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
