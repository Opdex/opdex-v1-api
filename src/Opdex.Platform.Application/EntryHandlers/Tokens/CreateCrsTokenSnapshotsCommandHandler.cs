using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class CreateCrsTokenSnapshotsCommandHandler : IRequestHandler<CreateCrsTokenSnapshotsCommand, Unit>
    {
        private readonly IMediator _mediator;
        
        public CreateCrsTokenSnapshotsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<Unit> Handle(CreateCrsTokenSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var crs = await _mediator.Send(new GetTokenByAddressQuery("CRS"), CancellationToken.None);
                    
            // Get active minute, hourly, daily snapshots if available based on DateTime.Now
            var snapshots = await _mediator.Send(new RetrieveActiveTokenSnapshotsByTokenIdQuery(crs.Id, request.BlockTime), CancellationToken.None);
            
            // Get current cmc price for strax
            var price = await _mediator.Send(new RetrieveCmcStraxPriceQuery(), CancellationToken.None);
            
            // Upsert CRS snapshots, minute, hourly, daily
            var snapshotTypes = new[] { SnapshotType.Minute, SnapshotType.Hourly, SnapshotType.Daily };
            foreach (var snapshotType in snapshotTypes)
            {
                var start = request.BlockTime;
                var end = start;

                switch (snapshotType)
                {
                    case SnapshotType.Minute:
                        start = start.StartOfMinute();
                        end = end.EndOfMinute();
                        break;
                    case SnapshotType.Hourly:
                        start = start.StartOfHour();
                        end = end.EndOfHour();
                        break;
                    case SnapshotType.Daily:
                        start = start.StartOfDay();
                        end = end.EndOfDay();
                        break;
                    default:
                        start = start.StartOfHour();
                        end = end.EndOfHour();
                        break;
                }

                // Todo: CRS is global but marketId is required...
                const long marketId = 0;
                
                var snapshot = snapshots.SingleOrDefault(s => s.SnapshotType == snapshotType) ??
                               new TokenSnapshot(crs.Id, marketId, price, snapshotType, start, end);

                if (snapshot.Id > 1)
                {
                    snapshot.UpdatePrice(price);
                }

                var persisted = await _mediator.Send(new MakeTokenSnapshotCommand(snapshot), CancellationToken.None);
                if (!persisted)
                {
                    throw new Exception("Unable to persist token snapshot.");
                }
            }
            
            return Unit.Value;
        }
    }
}