using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools
{
    public class SwapEventDtoAssembler : IModelAssembler<SwapLog, SwapEventDto>
    {
        private readonly IMediator _mediator;

        public SwapEventDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<SwapEventDto> Assemble(SwapLog log)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.Contract));
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));

            return new SwapEventDto
            {
                AmountCrsIn = log.AmountCrsIn.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                AmountSrcIn = log.AmountSrcIn.InsertDecimal(srcToken.Decimals),
                AmountCrsOut = log.AmountCrsOut.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                AmountSrcOut = log.AmountSrcOut.InsertDecimal(srcToken.Decimals),
                SrcToken = srcToken.Address,
                Sender = log.Sender,
                To = log.To,
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.SwapEvent
            };
        }
    }
}
