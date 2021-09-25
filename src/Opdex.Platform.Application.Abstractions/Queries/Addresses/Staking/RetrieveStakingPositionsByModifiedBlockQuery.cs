using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking
{
    public class RetrieveStakingPositionsByModifiedBlockQuery : IRequest<IEnumerable<AddressStaking>>
    {
        public RetrieveStakingPositionsByModifiedBlockQuery(ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
    }
}
