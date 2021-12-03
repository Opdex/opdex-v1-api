using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

/// <summary>
/// Calls Cirrus to get a block by its hash.
/// </summary>
public class CallCirrusGetBlockReceiptByHashQuery : FindQuery<BlockReceipt>
{
    /// <summary>
    /// Constructor to create a call cirrus get block receipt by hash query.
    /// </summary>
    /// <param name="hash">The block hash to retrieve the block by.</param>
    /// <param name="findOrThrow">Flag determining if no result is found, to throw not found or return null.</param>
    public CallCirrusGetBlockReceiptByHashQuery(Sha256 hash, bool findOrThrow = true) : base(findOrThrow)
    {
        Hash = hash;
    }

    public Sha256 Hash { get; }
}