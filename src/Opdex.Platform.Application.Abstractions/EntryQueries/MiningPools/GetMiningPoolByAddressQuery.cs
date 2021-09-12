using MediatR;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools
{
    /// <summary>
    /// Retrieves mining pool details from a mining pool address.
    /// </summary>
    public class GetMiningPoolByAddressQuery : IRequest<MiningPoolDto>
    {
        public GetMiningPoolByAddressQuery(Address miningPool)
        {
            MiningPool = miningPool != Address.Empty ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
        }

        public Address MiningPool { get; }
    }
}
