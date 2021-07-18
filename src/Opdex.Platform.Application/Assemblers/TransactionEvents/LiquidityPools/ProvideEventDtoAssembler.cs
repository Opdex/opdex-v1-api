using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools
{
    public class ProvideEventDtoAssembler<T> : IModelAssembler<T, ProvideEventDto>
        where T : TransactionLog
    {
        private readonly IMediator _mediator;

        public ProvideEventDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ProvideEventDto> Assemble(T log)
        {
            var isBurnLog = log.LogType == TransactionLogType.BurnLog;
            var isMintLog = log.LogType == TransactionLogType.MintLog;

            if (!isBurnLog && !isMintLog)
            {
                throw new ArgumentOutOfRangeException(nameof(log.LogType));
            }

            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(log.Contract));
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));
            var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId));

            return new ProvideEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.ProvideEvent,
                AmountCrs = isBurnLog
                    ? (log as BurnLog)?.AmountCrs.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals)
                    : (log as MintLog)?.AmountCrs.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                AmountSrc = isBurnLog
                    ? (log as BurnLog)?.AmountSrc.InsertDecimal(srcToken.Decimals)
                    : (log as MintLog)?.AmountSrc.InsertDecimal(srcToken.Decimals),
                AmountLpt = isBurnLog
                    ? (log as BurnLog)?.AmountLpt.InsertDecimal(lpToken.Decimals)
                    : (log as MintLog)?.AmountLpt.InsertDecimal(lpToken.Decimals),
                TokenLp = lpToken.Address,
                TokenSrc = srcToken.Address,
                SubEventType = isBurnLog ? "RemoveLiquidity" : "AddLiquidity"
            };
        }
    }
}
