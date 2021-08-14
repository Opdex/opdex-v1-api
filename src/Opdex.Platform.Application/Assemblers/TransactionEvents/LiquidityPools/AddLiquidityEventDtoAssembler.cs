using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools
{
    public class AddLiquidityEventDtoAssembler : IModelAssembler<MintLog, AddLiquidityEventDto>
    {
        private readonly IMediator _mediator;

        public AddLiquidityEventDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<AddLiquidityEventDto> Assemble(MintLog log)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.Contract));
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));

            return new AddLiquidityEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                AmountCrs = log.AmountCrs.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                AmountSrc = log.AmountSrc.InsertDecimal(srcToken.Decimals),
                AmountLpt = log.AmountLpt.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals),
                TokenLp = log.Contract,
                TokenSrc = srcToken.Address,
                TokenLpTotalSupply = log.TotalSupply
            };
        }
    }
}
