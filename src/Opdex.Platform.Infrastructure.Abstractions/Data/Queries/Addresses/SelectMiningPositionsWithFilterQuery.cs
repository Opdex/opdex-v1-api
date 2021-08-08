using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    /// <summary>
    /// Retrieves address mining collection based on provided filters
    /// </summary>
    public class SelectMiningPositionsWithFilterQuery : IRequest<IEnumerable<AddressMining>>
    {
        public SelectMiningPositionsWithFilterQuery(string address, MiningPositionsCursor cursor)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public string Address { get; }
        public MiningPositionsCursor Cursor { get; }
    }
}
