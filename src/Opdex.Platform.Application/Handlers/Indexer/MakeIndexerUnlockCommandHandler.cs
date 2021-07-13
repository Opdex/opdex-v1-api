using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Indexer
{
    public class MakeIndexerUnlockCommandHandler : IRequestHandler<MakeIndexerUnlockCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MakeIndexerUnlockCommandHandler> _logger;

        public MakeIndexerUnlockCommandHandler(IMediator mediator, ILogger<MakeIndexerUnlockCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(MakeIndexerUnlockCommand request, CancellationToken cancellationToken)
        {
            // Todo: Calling this command can overwrite another instance that locked indexing.
            // Considering using an identifier per instance to authorize who can persist the unlock flag
            var unlocked = await _mediator.Send(new PersistIndexerUnlockCommand());
            if (!unlocked)
            {
                _logger.LogCritical("Unable to unlock indexer.");
                throw new Exception("Indexer was unable to be unlocked.");
            }

            _logger.LogDebug("Indexer unlocked.");
            return Unit.Value;
        }
    }
}
