using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer
{
    public class PersistIndexerLockCommand : IRequest<bool>
    {
    }
}
