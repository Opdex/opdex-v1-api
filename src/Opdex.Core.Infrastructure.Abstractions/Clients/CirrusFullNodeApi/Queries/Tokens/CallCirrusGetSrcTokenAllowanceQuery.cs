using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenAllowanceQuery : IRequest<string>
    {
        public CallCirrusGetSrcTokenAllowanceQuery(string token, string owner, string spender)
        {
            Token = token.HasValue() ? token : throw new ArgumentNullException(nameof(token));
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner));
            Spender = spender.HasValue() ? spender : throw new ArgumentNullException(nameof(spender));
        }

        public string Token { get; }
        public string Owner { get; }
        public string Spender { get; }
    }
}