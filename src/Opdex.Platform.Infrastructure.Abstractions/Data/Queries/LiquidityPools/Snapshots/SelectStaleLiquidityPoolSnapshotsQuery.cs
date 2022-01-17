using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;

public class SelectStaleLiquidityPoolSnapshotsQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
{
}
