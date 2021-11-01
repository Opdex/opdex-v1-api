using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    /// <summary>
    /// Retrieves the hash of the block at a selected height.
    /// </summary>
    public class CallCirrusGetBlockHashByHeightQuery : IRequest<Sha256>
    {
        /// <summary>
        /// Creates a query to retrieve  the hash of the block at a selected height.
        /// </summary>
        /// <param name="height">The height of the block.</param>
        public CallCirrusGetBlockHashByHeightQuery(ulong height)
        {
            if (height < 1)
            {
                throw new ArgumentNullException(nameof(height));
            }

            Height = height;
        }

        public ulong Height { get; }
    }
}
