using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools;

public class GetLiquidityPoolsWithFilterQueryHandler : EntryFilterQueryHandler<GetLiquidityPoolsWithFilterQuery, LiquidityPoolsDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<LiquidityPool, LiquidityPoolDto> _assembler;
    private readonly ILogger<GetLiquidityPoolsWithFilterQueryHandler> _logger;

    public GetLiquidityPoolsWithFilterQueryHandler(IMediator mediator, IModelAssembler<LiquidityPool, LiquidityPoolDto> assembler, ILogger<GetLiquidityPoolsWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<LiquidityPoolsDto> Handle(GetLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var pools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(request.Cursor), cancellationToken);

        var dtos = await Task.WhenAll(pools.Select(pool => _assembler.Assemble(pool)));

        _logger.LogTrace("Assembled queried liquidity pools");

        var dtoResults = dtos.ToList();

        var cursor = BuildCursorDto(dtoResults, request.Cursor, pointerSelector: result =>
        {
            return request.Cursor.OrderBy switch
            {
                LiquidityPoolOrderByType.Liquidity => (result.Summary.Reserves.Usd.ToString(CultureInfo.InvariantCulture), result.Id),
                LiquidityPoolOrderByType.Volume => (result.Summary.Volume.DailyUsd.ToString(CultureInfo.InvariantCulture), result.Id),
                LiquidityPoolOrderByType.StakingWeight => (result.Summary.Staking?.Weight.ToString(), result.Id),
                LiquidityPoolOrderByType.Name => (result.Name, result.Id),
                _ => (string.Empty, result.Id)
            };
        });

        _logger.LogTrace("Returning {ResultCount} results", dtoResults.Count);

        return new LiquidityPoolsDto { LiquidityPools = dtoResults, Cursor = cursor };
    }
}
