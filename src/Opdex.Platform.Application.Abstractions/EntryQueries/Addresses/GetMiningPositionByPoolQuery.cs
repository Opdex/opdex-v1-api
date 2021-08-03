using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    /// <summary>
    /// Retrieves a mining position of a particular address in a mining pool
    /// /// </summary>
    public class GetMiningPositionByPoolQuery : IRequest<MiningPositionDto>
    {
        public GetMiningPositionByPoolQuery(string miner, string miningPool)
        {
            Address = miner.HasValue() ? miner : throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
            MiningPoolAddress = miningPool.HasValue() ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
        }

        public string Address { get; }
        public string MiningPoolAddress { get; }
    }
}
