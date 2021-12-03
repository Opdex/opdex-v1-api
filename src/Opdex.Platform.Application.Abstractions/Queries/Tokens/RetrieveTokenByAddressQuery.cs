using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens;

public class RetrieveTokenByAddressQuery : FindQuery<Token>
{
    public RetrieveTokenByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address), "Token address must be provided.");
        }

        Address = address;
    }

    public Address Address { get; }
}