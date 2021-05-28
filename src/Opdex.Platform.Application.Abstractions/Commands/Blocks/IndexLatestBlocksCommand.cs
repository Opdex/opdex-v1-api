using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Blocks
{
    public class IndexLatestBlocksCommand : IRequest<Unit>
    {
        public IndexLatestBlocksCommand(bool isDevelopEnv)
        {
            IsDevelopEnv = isDevelopEnv;
        }

        public bool IsDevelopEnv { get; }
    }
}