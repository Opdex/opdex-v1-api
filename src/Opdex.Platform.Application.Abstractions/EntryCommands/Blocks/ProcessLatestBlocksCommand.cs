using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks
{
    public class ProcessLatestBlocksCommand : IRequest<Unit>
    {
        public ProcessLatestBlocksCommand(bool isDevelopEnv)
        {
            IsDevelopEnv = isDevelopEnv;
        }
        
        public bool IsDevelopEnv { get; }
    }
}