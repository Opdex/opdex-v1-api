using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
    public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateBlockCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<bool> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
        {
            var blockCommand = new MakeBlockCommand(request.Height, request.Hash, request.Time, request.MedianTime);
            return _mediator.Send(blockCommand, CancellationToken.None);
        }
    }
}