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
    public class ProcessSrcTokenSnapshotCommandHandler : IRequestHandler<ProcessSrcTokenSnapshotCommand, decimal>
    {
        private readonly IMediator _mediator;

        public ProcessSrcTokenSnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<decimal> Handle(ProcessSrcTokenSnapshotCommand request, CancellationToken cancellationToken)
        {
            var tokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(request.SrcToken.Id,
                                                                                              request.MarketId,
                                                                                              request.BlockTime,
                                                                                              request.SnapshotType));
            // Update a stale snapshot if it is older than what was requested
            if (tokenSnapshot.EndDate < request.BlockTime)
            {
                var crsPerSrc = request.ReserveCrs.Token0PerToken1(request.ReserveSrc, request.SrcToken.Sats);
                tokenSnapshot.ResetStaleSnapshot(crsPerSrc, request.CrsUsd, request.BlockTime);
            }
            else
            {
                tokenSnapshot.UpdatePrice(request.ReserveCrs, request.ReserveSrc, request.CrsUsd, request.SrcToken.Sats);
            }

            await _mediator.Send(new MakeTokenSnapshotCommand(tokenSnapshot));

            return tokenSnapshot.Price.Close;
        }
    }
}
