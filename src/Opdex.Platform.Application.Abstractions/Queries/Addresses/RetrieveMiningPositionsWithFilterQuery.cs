using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    /// <summary>
    /// Retrieve address mining collection based on provided filters
    /// </summary>
    public class RetrieveMiningPositionsWithFilterQuery : IRequest<IEnumerable<AddressMining>>
    {
        public RetrieveMiningPositionsWithFilterQuery(Address address, MiningPositionsCursor cursor)
        {
            Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public Address Address { get; }
        public MiningPositionsCursor Cursor { get; }
    }
}
