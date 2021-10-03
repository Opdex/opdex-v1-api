using MediatR;
using Opdex.Platform.Domain.Models.Deployers;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Deployers
{
    public class RetrieveDeployersByModifiedBlockQuery : IRequest<IEnumerable<Deployer>>
    {
        public RetrieveDeployersByModifiedBlockQuery(ulong blockHeight)
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
