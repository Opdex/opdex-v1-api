using MediatR;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;

/// <summary>
/// Get latest block Opdex knows about. Start with our database, if we've never started syncing, get Cirrus chain tip
/// block receipt else lookup the block receipt from Cirrus of our latest db block.
/// </summary>
public class GetBestBlockReceiptQuery : IRequest<BlockReceipt>
{
}