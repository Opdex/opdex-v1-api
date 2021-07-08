using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances
{
    public class SelectMiningGovernanceByAddressQuery : FindQuery<MiningGovernance>
    {
        public SelectMiningGovernanceByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must not be null or empty.");
            }

            Address = address;
        }

        public string Address { get; }
    }
}
