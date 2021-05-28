using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks
{
    public class PersistIndexerLockCommand : IRequest<bool>
    {
    }
}