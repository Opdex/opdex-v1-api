using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools
{
    public class GetLiquidityPoolsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolDto>>
    {
        public GetLiquidityPoolsWithFilterQuery(Address marketAddress, bool? stakingEnabled, bool? miningEnabled, bool? nominated,
                                               uint skip, uint take, string sortBy, string orderBy, IEnumerable<Address> pools)
        {
            if (marketAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(marketAddress));
            }

            MarketAddress = marketAddress;
            Staking = stakingEnabled;
            Mining = miningEnabled;
            Nominated = nominated;
            Skip = skip;
            Take = take;
            SortBy = sortBy;
            OrderBy = orderBy;
            Pools = pools;
        }

        public Address MarketAddress { get; }
        public bool? Staking { get; }
        public bool? Mining { get; }
        public bool? Nominated { get; }
        public uint Skip { get; }
        public uint Take { get; }
        public string SortBy { get; }
        public string OrderBy { get; }
        public IEnumerable<Address> Pools { get; }
    }
}
