using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveMiningPoolByAddressQuery : FindQuery<MiningPool>
    {
        public RetrieveMiningPoolByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
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
