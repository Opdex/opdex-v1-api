using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Indexer.Application.Abstractions.Commands.Blocks;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Application.Handlers.Blocks
{
    public class MakeBlockCommandHandler : IRequestHandler<MakeBlockCommand, bool>
    {
        private readonly IMediator _mediator;
        
        public MakeBlockCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<bool> Handle(MakeBlockCommand request, CancellationToken cancellationToken)
        {
            var block = new Block(request.Height, request.Hash, request.Time, request.MedianTime);

            return await _mediator.Send(new PersistBlockCommand(block));
        }
    }
}