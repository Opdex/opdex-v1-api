using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Blocks;

public class MakeRewindToBlockCommandHandler : IRequestHandler<MakeRewindToBlockCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeRewindToBlockCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<bool> Handle(MakeRewindToBlockCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new ExecuteRewindToBlockCommand(request.Block));
    }
}