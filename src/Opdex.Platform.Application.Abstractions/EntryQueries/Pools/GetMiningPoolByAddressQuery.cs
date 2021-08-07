using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    /// <summary>
    /// Retrieves mining pool details from a mining pool address.
    /// </summary>
    public class GetMiningPoolByAddressQuery : IRequest<MiningPoolDto>
    {
        public GetMiningPoolByAddressQuery(string miningPool)
        {
            MiningPool = miningPool.HasValue() ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
        }

        public string MiningPool { get; }
    }
}
