using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens;

/// <summary>
/// Retrieve token attributes based on a token address
/// </summary>
public class RetrieveTokenAttributesByTokenAddressQuery : IRequest<IEnumerable<TokenAttribute>>
{
    /// <summary>
    /// Creates a query to retrieve the attributes of a token
    /// </summary>
    /// <param name="token">Address of the token</param>
    public RetrieveTokenAttributesByTokenAddressQuery(Address token)
    {
        if (token == Address.Empty) throw new ArgumentNullException(nameof(token), "Token address must be set");
        Token = token;
    }

    public Address Token { get; }
}
