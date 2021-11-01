using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    /// <summary>
    /// Retrieves the top 4 liquidity pools by staking weight.
    /// </summary>
    public class CallCirrusGetGovernanceNominationsSummaryQuery : IRequest<IEnumerable<GovernanceContractNominationSummary>>
    {
        /// <summary>
        /// Creates a query to retrieve the top 4 liquidity pools by staking weight.
        /// </summary>
        /// <param name="governance">The address of the governance contract.</param>
        /// <param name="blockHeight">The block height to search at.</param>
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
