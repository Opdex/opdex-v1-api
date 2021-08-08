using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetLiquidityPoolsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolDto>>
    {
        public GetLiquidityPoolsWithFilterQuery(string marketAddress, bool? stakingEnabled, bool? miningEnabled, bool? nominated,
                                               uint skip, uint take, string sortBy, string orderBy, IEnumerable<string> pools)
        {
            if (!marketAddress.HasValue())
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

        public string MarketAddress { get; }
        public bool? Staking { get; }
        public bool? Mining { get; }
        public bool? Nominated { get; }
        public uint Skip { get; }
        public uint Take { get; }
        public string SortBy { get; }
        public string OrderBy { get; }
        public IEnumerable<string> Pools { get; }
    }
}
