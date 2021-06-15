using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolSnapshotDto>>
    {
        public GetLiquidityPoolSnapshotsWithFilterQuery(string liquidityPoolAddress, DateTime? from, DateTime? to)
        {
            if (!liquidityPoolAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(liquidityPoolAddress));
            }

            From = from;
            To = to;
            LiquidityPoolAddress = liquidityPoolAddress;
        }

        public string LiquidityPoolAddress { get; }
        public DateTime? From { get; }
        public DateTime? To { get; }
    }
}