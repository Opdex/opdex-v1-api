using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenAllowanceQuery : IRequest<UInt256>
    {
        public CallCirrusGetSrcTokenAllowanceQuery(Address token, Address owner, Address spender)
        {
            Token = token;
            Owner = owner;
            Spender = spender;
        }

        public Address Token { get; }
        public Address Owner { get; }
        public Address Spender { get; }
    }
}
