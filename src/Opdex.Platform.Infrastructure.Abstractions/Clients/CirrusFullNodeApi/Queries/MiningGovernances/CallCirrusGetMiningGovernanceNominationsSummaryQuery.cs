using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernances
{
    /// <summary>
    /// Retrieves the top 4 liquidity pools by staking weight.
    /// </summary>
    public class CallCirrusGetMiningGovernanceNominationsSummaryQuery : IRequest<IEnumerable<MiningGovernanceContractNominationSummary>>
    {
        /// <summary>
        /// Creates a query to retrieve the top 4 liquidity pools by staking weight.
        /// </summary>
        /// <param name="miningGovernance">The address of the miningGovernance contract.</param>
        /// <param name="blockHeight">The block height to search at.</param>
        public CallCirrusGetMiningGovernanceNominationsSummaryQuery(Address miningGovernance, ulong blockHeight)
        {
            if (miningGovernance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningGovernance), "Mining governance address must be provided.");
            }

            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            MiningGovernance = miningGovernance;
            BlockHeight = blockHeight;
        }

        public Address MiningGovernance { get; }
        public ulong BlockHeight { get; }
    }
}
