using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    public class CallCirrusGetGovernanceNominationsSummaryQuery : IRequest<IEnumerable<GovernanceContractNominationSummary>>
    {
        public CallCirrusGetGovernanceNominationsSummaryQuery(Address governance, ulong blockHeight)
        {
            if (governance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(governance), "Governance address must be provided.");
            }

            if (blockHeight < 1)
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
