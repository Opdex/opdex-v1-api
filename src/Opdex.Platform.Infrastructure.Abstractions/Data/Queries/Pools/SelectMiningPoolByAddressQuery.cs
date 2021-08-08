using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectMiningPoolByAddressQuery : FindQuery<MiningPool>
    {
        public SelectMiningPoolByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }
        
        public string Address { get; }
    }
}