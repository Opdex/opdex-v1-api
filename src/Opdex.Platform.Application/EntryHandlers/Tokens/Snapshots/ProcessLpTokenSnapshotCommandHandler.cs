using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots
{
    public class ProcessLpTokenSnapshotCommandHandler : IRequestHandler<ProcessLpTokenSnapshotCommand, decimal>
    {
        private readonly IMediator _mediator;

        public ProcessLpTokenSnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<decimal> Handle(ProcessLpTokenSnapshotCommand request, CancellationToken cancellationToken)
        {
            // Prepare LP Token Snapshot
            var lptUsd = request.ReservesUsd / request.LpToken.TotalSupply.ToRoundedDecimal(request.LpToken.Decimals, request.LpToken.Decimals);

            var tokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(request.LpToken.Id,
                                                                                              request.MarketId,
                                                                                              request.BlockTime,
                                                                                              request.SnapshotType));
            // Update a stale snapshot if it is older than what was requested
            if (tokenSnapshot.EndDate < request.BlockTime)
            {
                tokenSnapshot.ResetStaleSnapshot(lptUsd, request.BlockTime);
            }
            else
            {
                tokenSnapshot.UpdatePrice(lptUsd);
            }

            await _mediator.Send(new MakeTokenSnapshotCommand(tokenSnapshot));

            return tokenSnapshot.Price.Close;
        }
    }
}