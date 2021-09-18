using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveMiningGovernanceContractSummaryByAddressQuery : IRequest<MiningGovernanceContractSummary>
    {
        public RetrieveMiningGovernanceContractSummaryByAddressQuery(Address governance, ulong blockHeight)
        {
            if (governance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(governance), "Governance address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Governance = governance;
            BlockHeight = blockHeight;
        }

        public Address Governance { get; }
        public ulong BlockHeight { get; }
    }
}
