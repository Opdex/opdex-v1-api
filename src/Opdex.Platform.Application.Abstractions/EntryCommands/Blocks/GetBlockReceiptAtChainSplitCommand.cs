using MediatR;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;

public class GetBlockReceiptAtChainSplitCommand : IRequest<BlockReceipt>
{
}
