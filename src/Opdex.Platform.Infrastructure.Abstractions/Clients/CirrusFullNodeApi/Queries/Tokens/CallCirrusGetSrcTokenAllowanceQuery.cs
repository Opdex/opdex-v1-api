using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenAllowanceQuery : IRequest<UInt256>
    {
        public CallCirrusGetSrcTokenAllowanceQuery(Address token, Address owner, Address spender)
        {
            Token = token != Address.Empty ? token : throw new ArgumentNullException(nameof(token));
            Owner = owner != Address.Empty ? owner : throw new ArgumentNullException(nameof(owner));
            Spender = spender != Address.Empty ? spender : throw new ArgumentNullException(nameof(spender));
        }

        public Address Token { get; }
        public Address Owner { get; }
        public Address Spender { get; }
    }
}
