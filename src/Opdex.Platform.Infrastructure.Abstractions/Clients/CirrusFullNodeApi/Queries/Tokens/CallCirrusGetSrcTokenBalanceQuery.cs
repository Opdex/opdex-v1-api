using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQuery : IRequest<UInt256>
    {
        public CallCirrusGetSrcTokenBalanceQuery(Address token, Address owner, ulong blockHeight)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Token address must be provided.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Token = token;
            Owner = owner;
            BlockHeight = blockHeight;
        }

        public Address Token { get; }
        public Address Owner { get; }
        public ulong BlockHeight { get; }
    }
}
