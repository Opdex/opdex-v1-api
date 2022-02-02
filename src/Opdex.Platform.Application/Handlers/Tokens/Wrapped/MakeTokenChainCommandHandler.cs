using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens.Wrapped;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Wrapped;

public class MakeTokenChainCommandHandler : IRequestHandler<MakeTokenChainCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeTokenChainCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeTokenChainCommand request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new PersistTokenChainCommand(request.Chain), CancellationToken.None);
    }
}
