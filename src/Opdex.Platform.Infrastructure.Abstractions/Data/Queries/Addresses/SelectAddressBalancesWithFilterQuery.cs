using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressBalancesWithFilterQuery : IRequest<IEnumerable<AddressBalance>>
    {
        public SelectAddressBalancesWithFilterQuery(string address, AddressBalancesCursor cursor)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public string Address { get; }
        public AddressBalancesCursor Cursor { get; }
    }
}
