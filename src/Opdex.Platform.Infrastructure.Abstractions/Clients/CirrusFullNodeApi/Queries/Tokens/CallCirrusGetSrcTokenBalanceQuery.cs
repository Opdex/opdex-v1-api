using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQuery : IRequest<decimal>
    {
        public CallCirrusGetSrcTokenBalanceQuery(string token, string owner, int decimals)
        {
            Token = token.HasValue() ? token : throw new ArgumentNullException(nameof(token));
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner));
            Decimals = decimals >= 0 ? decimals : throw new ArgumentOutOfRangeException(nameof(decimals), "Decimals must be greater or equal to 0");
        }
        
        public string Token { get; }
        public string Owner { get; }
        public int Decimals { get; }
    }
}