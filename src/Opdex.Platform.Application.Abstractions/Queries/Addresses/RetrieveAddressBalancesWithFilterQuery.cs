using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressBalancesWithFilterQuery : IRequest<IEnumerable<AddressBalance>>
    {
        public RetrieveAddressBalancesWithFilterQuery(string address, AddressBalancesCursor cursor)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public string Address { get; }
        public AddressBalancesCursor Cursor { get; }
    }
}
