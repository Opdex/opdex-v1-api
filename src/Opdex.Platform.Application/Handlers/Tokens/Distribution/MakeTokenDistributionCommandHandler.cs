using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Distribution;

public class MakeTokenDistributionCommandHandler : IRequestHandler<MakeTokenDistributionCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeTokenDistributionCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<bool> Handle(MakeTokenDistributionCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistTokenDistributionCommand(request.TokenDistribution), cancellationToken);
    }
}