using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes
{
    /// <summary>
    /// Retrieves the token input amount that would be required for a CRS-SRC or SRC-CRS swap transaction, given an exact token output amount.
    /// </summary>
    public class CallCirrusGetAmountInStandardQuoteQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a request that retrieves the token input amount that would be required for a CRS-SRC or SRC-CRS swap transaction, given an exact token output amount.
        /// </summary>
        /// <param name="router">The address of the router contract.</param>
        /// <param name="amountOut">The token output amount.</param>
        /// <param name="reserveIn">The reserve amount of the input token in the liquidity pool.</param>
        /// <param name="reserveOut">The reserve amount of the output token in the liquidity pool.</param>
        public CallCirrusGetAmountInStandardQuoteQuery(Address router, UInt256 amountOut, UInt256 reserveIn, UInt256 reserveOut)
        {
            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router), "The router address must be set.");
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
