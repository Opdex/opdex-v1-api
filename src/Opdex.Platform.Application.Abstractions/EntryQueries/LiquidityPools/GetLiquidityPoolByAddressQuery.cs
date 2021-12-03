using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;

public class GetLiquidityPoolByAddressQuery : IRequest<LiquidityPoolDto>
{
    public GetLiquidityPoolByAddressQuery(Address address)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address));
        }

        Address = address;
    }

    public Address Address { get; }
}