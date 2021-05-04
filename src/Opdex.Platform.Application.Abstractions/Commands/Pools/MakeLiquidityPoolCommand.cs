using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Pools
{
    public class MakeLiquidityPoolCommand : IRequest<long>
    {
        public MakeLiquidityPoolCommand(string address, long tokenId, long marketId)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }
            
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            Address = address;
            TokenId = tokenId;
            MarketId = marketId;
        }
        
        public string Address { get; }
        public long TokenId { get; }
        public long MarketId { get; set; }
    }
}