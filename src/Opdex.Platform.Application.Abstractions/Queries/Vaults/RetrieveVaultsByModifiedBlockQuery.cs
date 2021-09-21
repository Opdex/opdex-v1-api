using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    /// <summary>
    /// Retrieve all vaults based on their modified block.
    /// </summary>
    public class RetrieveVaultsByModifiedBlockQuery : IRequest<IEnumerable<Vault>>
    {
        /// <summary>
        /// Constructor to create a retrieve vaults by modified block query.
        /// </summary>
        /// <param name="blockHeight">The block height to select vaults by.</param>
        public RetrieveVaultsByModifiedBlockQuery(ulong blockHeight)
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
