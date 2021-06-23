using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets
{
    public class GetMarketSnapshotsWithFilterQuery : IRequest<IEnumerable<MarketSnapshotDto>>
    {
        public GetMarketSnapshotsWithFilterQuery(string marketAddress, DateTime? from, DateTime? to)
        {
            if (!marketAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(marketAddress), $"{nameof(marketAddress)} must be provided.");
            }

            MarketAddress = marketAddress;
            From = from;
            To = to;
        }

        public string MarketAddress { get; }
        public DateTime? From { get; }
        public DateTime? To { get; }
    }
}
