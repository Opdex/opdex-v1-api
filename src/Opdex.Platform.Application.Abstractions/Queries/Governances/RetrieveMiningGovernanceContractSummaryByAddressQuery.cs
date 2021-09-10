using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveMiningGovernanceContractSummaryByAddressQuery : IRequest<MiningGovernanceContractSummary>
    {
        public RetrieveMiningGovernanceContractSummaryByAddressQuery(Address address)
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
