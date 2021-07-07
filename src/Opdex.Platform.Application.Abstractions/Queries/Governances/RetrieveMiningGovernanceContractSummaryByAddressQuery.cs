using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.ODX;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveMiningGovernanceContractSummaryByAddressQuery : IRequest<MiningGovernanceContractSummary>
    {
        public RetrieveMiningGovernanceContractSummaryByAddressQuery(string address)
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
