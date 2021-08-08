using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveMiningGovernanceByAddressQuery : FindQuery<MiningGovernance>
    {
        public RetrieveMiningGovernanceByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
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
