using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools
{
    /// <summary>
    /// Retrieves the last calculated reward per LP token for the mining pool itself.
    /// </summary>
    public class CallCirrusGetMiningPoolRewardPerTokenMiningQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a request to retrieve the last calculated reward per LP token, for the mining pool itself.
        /// </summary>
        /// <param name="miningPool">The address of the mining pool.</param>
        /// <param name="blockHeight">The block height to search at.</param>
        public CallCirrusGetMiningPoolRewardPerTokenMiningQuery(Address miningPool, ulong blockHeight)
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
        }

        public Address MiningPool { get; }
        public ulong BlockHeight { get; }
    }
}
