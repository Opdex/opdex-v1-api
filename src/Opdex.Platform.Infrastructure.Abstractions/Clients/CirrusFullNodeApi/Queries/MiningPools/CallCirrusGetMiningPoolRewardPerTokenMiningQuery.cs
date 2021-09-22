using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools
{
    public class CallCirrusGetMiningPoolRewardPerTokenMiningQuery : IRequest<UInt256>
    {
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
