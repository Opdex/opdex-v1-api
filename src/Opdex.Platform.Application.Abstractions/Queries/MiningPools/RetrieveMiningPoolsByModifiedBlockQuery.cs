using MediatR;
using Opdex.Platform.Domain.Models.MiningPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningPools
{
    public class RetrieveMiningPoolsByModifiedBlockQuery : IRequest<IEnumerable<MiningPool>>
    {
        public RetrieveMiningPoolsByModifiedBlockQuery(ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
    }
}
