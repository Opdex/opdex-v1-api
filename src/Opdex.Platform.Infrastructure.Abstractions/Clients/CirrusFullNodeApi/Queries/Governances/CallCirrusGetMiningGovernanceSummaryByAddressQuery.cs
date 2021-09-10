using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    public class CallCirrusGetMiningGovernanceSummaryByAddressQuery : IRequest<MiningGovernanceContractSummary>
    {
        public CallCirrusGetMiningGovernanceSummaryByAddressQuery(Address address)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public Address Address { get; }
    }
}
