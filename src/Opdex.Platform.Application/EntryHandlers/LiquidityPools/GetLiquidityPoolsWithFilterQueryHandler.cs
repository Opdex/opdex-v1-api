using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools;

public class GetLiquidityPoolsWithFilterQueryHandler : EntryFilterQueryHandler<GetLiquidityPoolsWithFilterQuery, LiquidityPoolsDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<LiquidityPool, LiquidityPoolDto> _assembler;

    public GetLiquidityPoolsWithFilterQueryHandler(IMediator mediator, IModelAssembler<LiquidityPool, LiquidityPoolDto> assembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
    }

    public override async Task<LiquidityPoolsDto> Handle(GetLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var pools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(request.Cursor), cancellationToken);

        var dtos = await Task.WhenAll(pools.Select(pool => _assembler.Assemble(pool)));

        var dtoResults = dtos.ToList();

        var cursor = BuildCursorDto(dtoResults, request.Cursor, pointerSelector: result =>
        {
            return request.Cursor.OrderBy switch
            {
                LiquidityPoolOrderByType.Liquidity => (result.Summary.Reserves.Usd.ToString(), result.Id),
                LiquidityPoolOrderByType.Volume => (result.Summary.Volume.DailyUsd.ToString(), result.Id),
                LiquidityPoolOrderByType.StakingWeight => (result.Summary.Staking?.Weight.ToString(), result.Id),
                LiquidityPoolOrderByType.Name => (result.Name, result.Id),
                _ => (string.Empty, result.Id)
            };
        });

        return new LiquidityPoolsDto { LiquidityPools = dtoResults, Cursor = cursor };
    }
}