using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;

namespace Opdex.Platform.Application.Handlers.Blocks
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
            var unlocked = await _mediator.Send(new PersistIndexerUnlockCommand(), cancellationToken);
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