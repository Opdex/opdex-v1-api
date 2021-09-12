using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQuery : IRequest<UInt256>
    {
        public CallCirrusGetSrcTokenBalanceQuery(Address token, Address owner)
        {
            Token = token != Address.Empty ? token : throw new ArgumentNullException(nameof(token));
            Owner = owner != Address.Empty ? owner : throw new ArgumentNullException(nameof(owner));
        }

        public Address Token { get; }
        public Address Owner { get; }
    }
}
