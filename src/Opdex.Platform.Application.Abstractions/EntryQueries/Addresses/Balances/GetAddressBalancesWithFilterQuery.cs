using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;

public class GetAddressBalancesWithFilterQuery : IRequest<AddressBalancesDto>
{
    public GetAddressBalancesWithFilterQuery(Address address, AddressBalancesCursor cursor)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public Address Address { get; }
    public AddressBalancesCursor Cursor { get; }
}