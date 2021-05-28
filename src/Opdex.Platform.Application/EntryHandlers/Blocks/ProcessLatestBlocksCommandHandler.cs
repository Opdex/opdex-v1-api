using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
    public class ProcessLatestBlocksCommandHandler : IRequestHandler<ProcessLatestBlocksCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessLatestBlocksCommandHandler> _logger;

        public ProcessLatestBlocksCommandHandler(IMediator mediator, ILogger<ProcessLatestBlocksCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(ProcessLatestBlocksCommand request, CancellationToken cancellationToken)
        {
            var lockCreated = await _mediator.Send(new PersistIndexerLockCommand(), cancellationToken);
            if (!lockCreated) throw new IndexingAlreadyRunningException();

            _logger.LogDebug("Indexer locked");

            try
            {
                await _mediator.Send(new IndexLatestBlocksCommand(request.IsDevelopEnv), cancellationToken);
            }
            finally
            {
                await _mediator.Send(new PersistIndexerUnlockCommand());
                _logger.LogDebug("Indexer unlocked");
            }

            return Unit.Value;
        }
    }
}