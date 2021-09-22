using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets
{
    public class GetMarketSnapshotsWithFilterQuery : IRequest<IEnumerable<MarketSnapshotDto>>
    {
        public GetMarketSnapshotsWithFilterQuery(Address marketAddress, DateTime? from, DateTime? to)
        {
            if (marketAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(marketAddress), $"{nameof(marketAddress)} must be provided.");
            }

            MarketAddress = marketAddress;
            From = from;
            To = to;
        }

        public Address MarketAddress { get; }
        public DateTime? From { get; }
        public DateTime? To { get; }
    }
}
