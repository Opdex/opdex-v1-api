using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    /// <summary>
    /// Retrieves staking position collection based on the provided filters
    /// </summary>
    public class GetStakingPositionsWithFilterQuery : IRequest<StakingPositionsDto>
    {
        public GetStakingPositionsWithFilterQuery(string address, StakingPositionsCursor cursor)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public string Address { get; }
        public StakingPositionsCursor Cursor { get; }
    }
}
