using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Attributes;

public class MakeTokenAttributeCommandHandler : IRequestHandler<MakeTokenAttributeCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeTokenAttributeCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeTokenAttributeCommand request, CancellationToken cancellationToken)
    {
        var attributes = await _mediator.Send(new SelectTokenAttributesByTokenIdQuery(request.TokenAttribute.TokenId));

        if (attributes.All(a => a.AttributeType != (request.TokenAttribute.AttributeType)))
        {
            return await _mediator.Send(new PersistTokenAttributeCommand(request.TokenAttribute));
        }

        return true;
    }
}