using MediatR;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

/// <summary>
/// Call cirrus to get the block hash of the chain tip.
/// </summary>
public class CallCirrusGetBestBlockReceiptQuery : IRequest<BlockReceipt>
{
}