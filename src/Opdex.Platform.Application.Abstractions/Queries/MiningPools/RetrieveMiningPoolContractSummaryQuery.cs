using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningPools
{
    /// <summary>
    /// Retrieve select properties from a mining pool smart contract based on the provided block height.
    /// </summary>
    public class RetrieveMiningPoolContractSummaryQuery : IRequest<MiningPoolContractSummary>
    {
        /// <summary>
        /// Constructor to create a retrieve mining pool contract summary query.
        /// </summary>
        /// <param name="miningPool">The address of the mining pool contract.</param>
        /// <param name="blockHeight">The block height to query the contract's state at.</param>
        /// <param name="includeRewardPerBlock">Flag to include the mining pool's reward per block property, default is false.</param>
        /// <param name="includeRewardPerLpt">Flag to include the mining pool's reward per liquidity pool token property, default is false.</param>
        /// <param name="includeMiningPeriodEndBlock">Flag to include the mining pool's mining period end block property, default is false.</param>
        public RetrieveMiningPoolContractSummaryQuery(Address miningPool, ulong blockHeight, bool includeRewardPerBlock = false,
                                                      bool includeRewardPerLpt = false, bool includeMiningPeriodEndBlock = false)
        {
            if (miningPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            MiningPool = miningPool;
            BlockHeight = blockHeight;
            IncludeRewardPerBlock = includeRewardPerBlock;
            IncludeRewardPerLpt = includeRewardPerLpt;
            IncludeMiningPeriodEndBlock = includeMiningPeriodEndBlock;
        }

        public Address MiningPool { get; }
        public ulong BlockHeight { get; }
        public bool IncludeRewardPerBlock { get; }
        public bool IncludeRewardPerLpt { get; }
        public bool IncludeMiningPeriodEndBlock { get; }
    }
}
