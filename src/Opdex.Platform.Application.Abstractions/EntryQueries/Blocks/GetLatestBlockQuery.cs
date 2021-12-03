using MediatR;
using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;

/// <summary>
/// Retrieves the latest indexed block.
/// </summary>
public class GetLatestBlockQuery : IRequest<BlockDto>
{
}