using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;

/// <summary>
/// Query to select attributes for a token
/// </summary>
public class SelectTokenAttributesByTokenAddressQuery : IRequest<IEnumerable<TokenAttribute>>
{
    /// <summary>
    /// Creates a query to select attributes for a token
    /// </summary>
    /// <param name="token">Address of the token</param>
    public SelectTokenAttributesByTokenAddressQuery(Address token)
    {
        if (token == Address.Empty) throw new ArgumentNullException(nameof(token), "Token address must be set");
        Token = token;
    }

    public Address Token { get; }
}
