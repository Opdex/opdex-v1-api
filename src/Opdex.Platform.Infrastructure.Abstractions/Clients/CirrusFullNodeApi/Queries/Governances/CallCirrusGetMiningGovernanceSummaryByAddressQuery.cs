using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.ODX;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    public class CallCirrusGetMiningGovernanceSummaryByAddressQuery : IRequest<MiningGovernanceContractSummary>
    {
        public CallCirrusGetMiningGovernanceSummaryByAddressQuery(string address)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public string Address { get; }
    }
}
