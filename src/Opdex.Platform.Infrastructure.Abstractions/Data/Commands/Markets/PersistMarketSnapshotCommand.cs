using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets
{
    public class PersistMarketSnapshotCommand : IRequest<bool>
    {
        public PersistMarketSnapshotCommand()
        {
            
        }
    }
}