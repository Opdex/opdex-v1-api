using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Indexer;

public class MakeIndexerLockReasonCommandHandler : AsyncRequestHandler<MakeIndexerLockReasonCommand>
{
    private readonly IMediator _mediator;

    public MakeIndexerLockReasonCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override async Task Handle(MakeIndexerLockReasonCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new PersistIndexerLockCommand(request.Reason), cancellationToken);
    }
}
