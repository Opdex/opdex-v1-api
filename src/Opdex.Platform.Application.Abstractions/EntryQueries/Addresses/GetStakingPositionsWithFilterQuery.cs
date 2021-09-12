using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    /// <summary>
    /// Retrieves staking position collection based on the provided filters
    /// </summary>
    public class GetStakingPositionsWithFilterQuery : IRequest<StakingPositionsDto>
    {
        public GetStakingPositionsWithFilterQuery(Address address, StakingPositionsCursor cursor)
        {
            Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public Address Address { get; }
        public StakingPositionsCursor Cursor { get; }
    }
}
