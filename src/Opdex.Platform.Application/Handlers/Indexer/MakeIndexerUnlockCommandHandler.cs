using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Indexer;

public class MakeIndexerUnlockCommandHandler : AsyncRequestHandler<MakeIndexerUnlockCommand>
{
    private readonly IMediator _mediator;

    public MakeIndexerUnlockCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override async Task Handle(MakeIndexerUnlockCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new PersistIndexerUnlockCommand(), CancellationToken.None);
    }
}