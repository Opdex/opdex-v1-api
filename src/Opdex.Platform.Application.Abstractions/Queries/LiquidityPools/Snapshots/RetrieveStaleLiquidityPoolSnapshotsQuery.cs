using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;

public class RetrieveStaleLiquidityPoolSnapshotsQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
{
}
