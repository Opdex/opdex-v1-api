using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking
{
    /// <summary>
    /// Retrieves address staking collection based on the provided filters
    /// </summary>
    public class RetrieveStakingPositionsWithFilterQuery : IRequest<IEnumerable<AddressStaking>>
    {
        public RetrieveStakingPositionsWithFilterQuery(Address address, StakingPositionsCursor cursor)
        {
            Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public Address Address { get; }
        public StakingPositionsCursor Cursor { get; }
    }
}
