using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    /// <summary>
    /// Retrieves the token output amount that would be required for a CRS-SRC or SRC-CRS swap transaction, given an exact token input amount.
    /// </summary>
    public class CallCirrusGetAmountOutStandardQuoteQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a request that retrieves the token output amount that would be required for a CRS-SRC or SRC-CRS swap transaction, given an exact token input amount.
        /// </summary>
        /// <param name="router">The address of the router contract.</param>
        /// <param name="amountIn">The token input amount.</param>
        /// <param name="reserveIn">The reserve amount of the input token in the liquidity pool.</param>
        /// <param name="reserveOut">The reserve amount of the output token in the liquidity pool.</param>
        public CallCirrusGetAmountOutStandardQuoteQuery(Address router, UInt256 amountIn, UInt256 reserveIn, UInt256 reserveOut)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            if (amountIn == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountIn), "Amount in must be greater than 0.");
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
