using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;

/// <summary>
/// Retrieve mining position collection based on provided filters
/// </summary>
public class GetMiningPositionsWithFilterQuery : IRequest<MiningPositionsDto>
{
    public GetMiningPositionsWithFilterQuery(Address address, MiningPositionsCursor cursor)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public Address Address { get; }
    public MiningPositionsCursor Cursor { get; }
}