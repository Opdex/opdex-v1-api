using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressBalancesWithFilterQuery : IRequest<AddressBalancesDto>
    {
        public GetAddressBalancesWithFilterQuery(string address, AddressBalancesCursor cursor)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public string Address { get; }
        public AddressBalancesCursor Cursor { get; }
    }
}
