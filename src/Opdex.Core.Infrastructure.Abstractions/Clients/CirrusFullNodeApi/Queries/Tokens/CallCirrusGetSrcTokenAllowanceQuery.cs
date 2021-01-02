using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenAllowanceQuery : IRequest<decimal>
    {
        public CallCirrusGetSrcTokenAllowanceQuery(string token, string owner, string spender, int decimals)
        {
            Token = token.HasValue() ? token : throw new ArgumentNullException(nameof(token));
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner));
            Spender = spender.HasValue() ? spender : throw new ArgumentNullException(nameof(spender));
            Decimals = decimals >= 0 ? decimals : throw new ArgumentOutOfRangeException(nameof(decimals), "Decimals must be greater or equal to 0");
        }

        public string Token { get; }
        public string Owner { get; }
        public string Spender { get; }
        public int Decimals { get; }
    }
}