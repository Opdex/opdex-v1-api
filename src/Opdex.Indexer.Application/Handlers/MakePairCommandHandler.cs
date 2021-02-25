using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Application.Handlers
{
    public class MakePairCommandHandler : IRequestHandler<MakePairCommand, bool>
    {
        private readonly IMediator _mediator;

        public MakePairCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<bool> Handle(MakePairCommand request, CancellationToken cancellationToken)
        {
            // Call Cirrus get pair details
            // Persist Pair
            throw new NotImplementedException();
        }
    }
}