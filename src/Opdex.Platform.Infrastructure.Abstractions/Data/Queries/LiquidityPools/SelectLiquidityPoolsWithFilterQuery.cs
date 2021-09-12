using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools
{
    public class SelectLiquidityPoolsWithFilterQuery : IRequest<IEnumerable<LiquidityPool>>
    {
        public SelectLiquidityPoolsWithFilterQuery(long marketId, bool? stakingEnabled = null, bool? miningEnabled = null, bool? nominated = null,
                                                   uint skip = 0, uint take = 0, string sortBy = null, string orderBy = null, IEnumerable<Address> pools = null)
        {
            if (marketId < 1)
            {
                throw new ArgumentNullException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            MarketId = marketId;
            Staking = stakingEnabled;
            Mining = miningEnabled;
            Nominated = nominated;
            Skip = skip;
            Take = take;
            SortBy = sortBy;
            OrderBy = orderBy;
            Pools = pools ?? Enumerable.Empty<Address>();
        }

        public long MarketId { get; }
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
