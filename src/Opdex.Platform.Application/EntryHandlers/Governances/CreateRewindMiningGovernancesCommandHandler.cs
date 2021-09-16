using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class CreateRewindMiningGovernancesCommandHandler : IRequestHandler<CreateRewindMiningGovernancesCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindMiningGovernancesCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindMiningGovernancesCommand request, CancellationToken cancellationToken)
        {
            var governances = await _mediator.Send(new RetrieveMiningGovernancesByModifiedBlockQuery(request.RewindHeight));

            // Split the governances by token into chunks of 10
            var balanceChunks = governances.Chunk(10);

            foreach (var chunk in balanceChunks)
            {
                // Each chunk runs in parallel
                var tasks = chunk.Select(governance =>_mediator.Send(new MakeMiningGovernanceCommand(governance, request.RewindHeight, rewind: true)));

                await Task.WhenAll(tasks);
            }

            return true;
        }
    }
}
