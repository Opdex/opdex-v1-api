using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;

public class RetrieveAddressBalancesWithFilterQuery : IRequest<IEnumerable<AddressBalance>>
{
    public RetrieveAddressBalancesWithFilterQuery(Address address, AddressBalancesCursor cursor)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public Address Address { get; }
    public AddressBalancesCursor Cursor { get; }
}