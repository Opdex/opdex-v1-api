using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetMiningPoolByTokenQuery : IRequest<string>
    {
        public CallCirrusGetMiningPoolByTokenQuery(string liquidityPoolAddress, ulong blockHeight)
        {
            if (!liquidityPoolAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPoolAddress), "Liquidity pool address must be set.");
            }

            LiquidityPoolAddress = liquidityPoolAddress;
            BlockHeight = blockHeight;
        }

        public string LiquidityPoolAddress { get; }
        public ulong BlockHeight { get; }
    }
}
