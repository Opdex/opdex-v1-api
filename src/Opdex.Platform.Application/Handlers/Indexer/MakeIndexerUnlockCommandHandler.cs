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
            var unlocked = await _mediator.Send(new PersistIndexerUnlockCommand(), CancellationToken.None);
            if (!unlocked)
            {
                _logger.LogCritical("Unable to unlock indexer.");
            }

            _logger.LogDebug("Indexer unlocked.");
            return Unit.Value;
        }
    }
}
