using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQuery : IRequest<string>
    {
        public CallCirrusGetSrcTokenBalanceQuery(string token, string owner)
        {
            Token = token.HasValue() ? token : throw new ArgumentNullException(nameof(token));
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner));
        }
        
        public string Token { get; }
        public string Owner { get; }
    }
}