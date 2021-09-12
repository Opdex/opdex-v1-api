using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolByAddressQuery : FindQuery<LiquidityPool>
    {
        public RetrieveLiquidityPoolByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public Address Address { get; }
    }
}
