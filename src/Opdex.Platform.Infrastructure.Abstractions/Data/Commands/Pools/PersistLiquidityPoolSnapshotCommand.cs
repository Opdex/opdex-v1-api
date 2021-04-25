using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools
{
    public class PersistLiquidityPoolSnapshotCommand : IRequest<long>
    {
        public PersistLiquidityPoolSnapshotCommand()
        {
            
        }
    }
}