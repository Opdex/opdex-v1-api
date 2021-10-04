using MediatR;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    /// <summary>
    /// Retrieve the latest block receipt from Cirrus, its current chain tip.
    /// </summary>
    public class RetrieveCirrusBestBlockReceiptQuery : IRequest<BlockReceipt>
    {
    }
}
