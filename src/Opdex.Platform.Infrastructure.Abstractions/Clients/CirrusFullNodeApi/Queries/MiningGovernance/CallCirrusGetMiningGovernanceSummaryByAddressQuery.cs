using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernance
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