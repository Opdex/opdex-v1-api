using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Deployers
{
    public class CreateRewindDeployersCommandHandler : IRequestHandler<CreateRewindDeployersCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindDeployersCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindDeployersCommand request, CancellationToken cancellationToken)
        {
            var deployers = await _mediator.Send(new RetrieveDeployersByModifiedBlockQuery(request.RewindHeight));

            // Split the address balances by token into chunks of 10
            var balanceChunks = deployers.Chunk(10);

            foreach (var chunk in balanceChunks)
            {
                // Each chunk runs in parallel
                var tasks = chunk.Select(deployer =>
                {
                    deployer.RequireRewind();
                    return _mediator.Send(new MakeDeployerCommand(deployer, request.RewindHeight));
                });

                await Task.WhenAll(tasks);
            }

            return true;
        }
    }
}
