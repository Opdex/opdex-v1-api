using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools;

public class RemoveLiquidityEventDtoAssembler : IModelAssembler<BurnLog, RemoveLiquidityEventDto>
{
    private readonly IMediator _mediator;

    public RemoveLiquidityEventDtoAssembler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<RemoveLiquidityEventDto> Assemble(BurnLog log)
    {
        var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.Contract));
        var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));

        return new RemoveLiquidityEventDto
        {
            Id = log.Id,
            TransactionId = log.TransactionId,
            SortOrder = log.SortOrder,
            Contract = log.Contract,
            AmountCrs = log.AmountCrs.ToDecimal(TokenConstants.Cirrus.Decimals),
            AmountSrc = log.AmountSrc.ToDecimal(srcToken.Decimals),
            AmountLpt = log.AmountLpt.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals),
            TokenLp = log.Contract,
            TokenSrc = srcToken.Address,
            TokenLpTotalSupply = log.TotalSupply.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)
        };
    }
}