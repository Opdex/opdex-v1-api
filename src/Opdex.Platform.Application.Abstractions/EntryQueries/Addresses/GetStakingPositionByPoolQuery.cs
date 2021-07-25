using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetStakingPositionByPoolQuery : IRequest<StakingPositionDto>
    {
        public GetStakingPositionByPoolQuery(string staker, string liquidityPool)
        {
            Address = staker.HasValue() ? staker : throw new ArgumentNullException(nameof(staker), "Staker address must be set.");
            LiquidityPoolAddress = liquidityPool.HasValue() ? liquidityPool : throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool address must be set.");
        }

        public string Address { get; }
        public string LiquidityPoolAddress { get; }
    }
}
