using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Indexer
{
    public class MakeIndexerLockCommandHandler : IRequestHandler<MakeIndexerLockCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MakeIndexerLockCommandHandler> _logger;

        public MakeIndexerLockCommandHandler(IMediator mediator, ILogger<MakeIndexerLockCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public async Task<Unit> Handle(MakeIndexerLockCommand request, CancellationToken cancellationToken)
        {
            var lockCreated = await _mediator.Send(new PersistIndexerLockCommand(), cancellationToken);
            if (!lockCreated)
            {
                throw new IndexingAlreadyRunningException();
            }

            _logger.LogDebug("Indexer locked");

            return Unit.Value;
        }
    }
}
