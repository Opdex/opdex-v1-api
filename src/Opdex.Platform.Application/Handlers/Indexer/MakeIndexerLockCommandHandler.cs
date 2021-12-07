using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Indexer;

public class MakeIndexerLockCommandHandler : IRequestHandler<MakeIndexerLockCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeIndexerLockCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeIndexerLockCommand request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new PersistIndexerLockCommand(request.Reason), cancellationToken);
    }
}
