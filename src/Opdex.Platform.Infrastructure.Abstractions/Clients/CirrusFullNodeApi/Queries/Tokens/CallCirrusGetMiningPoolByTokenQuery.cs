using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetMiningPoolByTokenQuery : IRequest<Address>
    {
        public CallCirrusGetMiningPoolByTokenQuery(Address liquidityPoolAddress, ulong blockHeight)
        {
            if (liquidityPoolAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPoolAddress), "Liquidity pool address must be set.");
            }

            LiquidityPoolAddress = liquidityPoolAddress;
            BlockHeight = blockHeight;
        }

        public Address LiquidityPoolAddress { get; }
        public ulong BlockHeight { get; }
    }
}
