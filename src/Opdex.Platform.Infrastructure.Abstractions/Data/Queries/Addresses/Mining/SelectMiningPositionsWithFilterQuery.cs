using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining
{
    /// <summary>
    /// Retrieves address mining collection based on provided filters
    /// </summary>
    public class SelectMiningPositionsWithFilterQuery : IRequest<IEnumerable<AddressMining>>
    {
        public SelectMiningPositionsWithFilterQuery(Address address, MiningPositionsCursor cursor)
        {
            Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public Address Address { get; }
        public MiningPositionsCursor Cursor { get; }
    }
}
