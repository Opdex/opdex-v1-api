using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    /// <summary>
    /// Retrieve select properties from a staking token smart contract based on the provided block height.
    /// </summary>
    public class RetrieveStakingTokenContractSummaryQuery : IRequest<StakingTokenContractSummary>
    {
        /// <summary>
        /// Constructor to create a retrieve staking token contract summary query.
        /// </summary>
        /// <param name="token">The address of the token contract.</param>
        /// <param name="blockHeight">The block height to query the contract's state at.</param>
        /// <param name="includeGenesis">Flag to include the token's genesis block property, default is false.</param>
        /// <param name="includePeriodIndex">Flag to include the token's period index property, default is false.</param>
        /// <param name="includeVault">Flag to include the token's vault address property, default is false.</param>
        /// <param name="includeMiningGovernance">Flag to include the token's mining governance address property, default is false.</param>
        /// <param name="includePeriodDuration">Flag to include the token's period duration property, default is false.</param>
        public RetrieveStakingTokenContractSummaryQuery(Address token, ulong blockHeight, bool includeGenesis = false,
                                                        bool includePeriodIndex = false, bool includeVault = false,
                                                        bool includeMiningGovernance = false, bool includePeriodDuration = false)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Token = token;
            BlockHeight = blockHeight;
            IncludeGenesis = includeGenesis;
            IncludePeriodIndex = includePeriodIndex;
            IncludeVault = includeVault;
            IncludeMiningGovernance = includeMiningGovernance;
            IncludePeriodDuration = includePeriodDuration;
        }

        public Address Token { get; }
        public ulong BlockHeight { get; }
        public bool IncludeGenesis { get; }
        public bool IncludePeriodIndex { get; }
        public bool IncludeVault { get; }
        public bool IncludeMiningGovernance { get; }
        public bool IncludePeriodDuration { get; }
    }
}
