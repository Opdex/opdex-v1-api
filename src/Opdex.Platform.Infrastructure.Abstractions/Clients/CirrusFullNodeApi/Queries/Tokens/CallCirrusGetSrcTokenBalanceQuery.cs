using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    /// <summary>
    /// Retrieves an SRC token balance of an address.
    /// </summary>
    public class CallCirrusGetSrcTokenBalanceQuery : IRequest<UInt256>
    {
        /// <summary>
        /// Creates a query to retrieve an SRC token balance.
        /// </summary>
        /// <param name="token">The address of the SRC token.</param>
        /// <param name="owner">The address of the balance holder.</param>
        /// <param name="blockHeight">The block height to retrieve at.</param>
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
