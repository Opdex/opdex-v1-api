using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    /// <summary>
    /// Retrieve select properties from a mining governance smart contract based on the provided block height.
    /// </summary>
    public class RetrieveMiningGovernanceContractSummaryByAddressQuery : IRequest<MiningGovernanceContractSummary>
    {
        /// <summary>
        /// Constructor to create a retrieve mining governance contract summary by address query.
        /// </summary>
        /// <param name="governance">The address of the mining governance contract.</param>
        /// <param name="blockHeight">The block height to query the contract's state at.</param>
        /// <param name="includeNominationPeriodEnd">Flag to include the governance nomination period end property, default is false.</param>
        /// <param name="includeMiningPoolsFunded">Flag to include the governance mining pools funded property, default is false.</param>
        /// <param name="includeMiningPoolReward">Flag to include the governance mining pool reward property, default is false.</param>
        /// <param name="includeMiningDuration">Flag to include the governance mining duration property, default is false.</param>
        /// <param name="includeMinedToken">Flag to include the governance mined token property, default is false.</param>
        public RetrieveMiningGovernanceContractSummaryByAddressQuery(Address governance, ulong blockHeight, bool includeNominationPeriodEnd = false,
                                                                     bool includeMiningPoolsFunded = false, bool includeMiningPoolReward = false,
                                                                     bool includeMiningDuration = false, bool includeMinedToken = false)
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
            IncludeNominationPeriodEnd = includeNominationPeriodEnd;
            IncludeMiningPoolsFunded = includeMiningPoolsFunded;
            IncludeMiningPoolReward = includeMiningPoolReward;
            IncludeMiningDuration = includeMiningDuration;
            IncludeMinedToken = includeMinedToken;
        }

        public Address Governance { get; }
        public ulong BlockHeight { get; }
        public bool IncludeNominationPeriodEnd { get; }
        public bool IncludeMiningPoolsFunded { get; }
        public bool IncludeMiningPoolReward { get; }
        public bool IncludeMiningDuration { get; }
        public bool IncludeMinedToken { get; }
    }
}
