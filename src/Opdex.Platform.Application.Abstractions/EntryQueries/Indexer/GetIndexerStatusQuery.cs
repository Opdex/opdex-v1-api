using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Index;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;

public class GetIndexerStatusQuery : IRequest<IndexerStatusDto>
{
}

