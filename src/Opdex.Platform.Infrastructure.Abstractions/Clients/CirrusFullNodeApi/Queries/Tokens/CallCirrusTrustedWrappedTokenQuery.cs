using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

/// <summary>
/// Queries Cirrus node to check whether an interflux token is trusted
/// </summary>
public class CallCirrusTrustedWrappedTokenQuery : IRequest<bool>
{
    /// <summary>
    /// Creates a query to query Cirrus, to check whether an interflux token is trusted
    /// </summary>
    /// <param name="token">Address of the SRC-20 token</param>
    public CallCirrusTrustedWrappedTokenQuery(Address token)
    {
        if (token == Address.Empty) throw new ArgumentNullException(nameof(token));
        Token = token;
    }

    public Address Token { get; }
}
