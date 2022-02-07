using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools;

public class ReservesChangeEventDtoAssembler : IModelAssembler<ReservesLog, ReservesChangeEventDto>
{
    private readonly IMediator _mediator;

    public ReservesChangeEventDtoAssembler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ReservesChangeEventDto> Assemble(ReservesLog log)
    {
        var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.Contract), CancellationToken.None);
        var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId), CancellationToken.None);

        return new ReservesChangeEventDto
        {
            Id = log.Id,
            TransactionId = log.TransactionId,
            Contract = log.Contract,
            SortOrder = log.SortOrder,
            Crs = log.ReserveCrs.ToDecimal(TokenConstants.Cirrus.Decimals),
            Src = log.ReserveSrc.ToDecimal(srcToken.Decimals)
        };
    }
}
