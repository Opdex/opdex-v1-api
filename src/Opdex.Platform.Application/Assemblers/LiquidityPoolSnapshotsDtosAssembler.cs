using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class LiquidityPoolSnapshotsDtosAssembler : IModelAssembler<IList<LiquidityPoolSnapshot>, IEnumerable<LiquidityPoolSnapshotDto>>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public LiquidityPoolSnapshotsDtosAssembler(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<LiquidityPoolSnapshotDto>> Assemble(IList<LiquidityPoolSnapshot> snapshots)
    {
        if (snapshots == null || !snapshots.Any()) return Enumerable.Empty<LiquidityPoolSnapshotDto>();

        var pool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(snapshots.First().LiquidityPoolId));
        var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId));

        return snapshots.Select(snapshot => new LiquidityPoolSnapshotDto
        {
            Id = snapshot.Id,
            LiquidityPoolId = snapshot.LiquidityPoolId,
            TransactionCount = snapshot.TransactionCount,
            Reserves = new ReservesSnapshotDto
            {
                Crs = new OhlcDto<FixedDecimal>
                {
                    Open = snapshot.Reserves.Crs.Open.ToDecimal(TokenConstants.Cirrus.Decimals),
                    High = snapshot.Reserves.Crs.High.ToDecimal(TokenConstants.Cirrus.Decimals),
                    Low = snapshot.Reserves.Crs.Low.ToDecimal(TokenConstants.Cirrus.Decimals),
                    Close = snapshot.Reserves.Crs.Close.ToDecimal(TokenConstants.Cirrus.Decimals)
                },
                Src = new OhlcDto<FixedDecimal>
                {
                    Open = snapshot.Reserves.Src.Open.ToDecimal(srcToken.Decimals),
                    High = snapshot.Reserves.Src.High.ToDecimal(srcToken.Decimals),
                    Low = snapshot.Reserves.Src.Low.ToDecimal(srcToken.Decimals),
                    Close = snapshot.Reserves.Src.Close.ToDecimal(srcToken.Decimals)
                },
                Usd = _mapper.Map<OhlcDto<decimal>>(snapshot.Reserves.Usd)
            },
            Rewards = _mapper.Map<RewardsSnapshotDto>(snapshot.Rewards),
            Staking = _mapper.Map<StakingSnapshotDto>(snapshot.Staking),
            Volume = new VolumeSnapshotDto
            {
                Crs = snapshot.Volume.Crs.ToDecimal(TokenConstants.Cirrus.Decimals),
                Src = snapshot.Volume.Src.ToDecimal(srcToken.Decimals),
                Usd = snapshot.Volume.Usd
            },
            Cost = new CostSnapshotDto
            {
                CrsPerSrc = new OhlcDto<FixedDecimal>
                {
                    Open = snapshot.Cost.CrsPerSrc.Open.ToDecimal(TokenConstants.Cirrus.Decimals),
                    High = snapshot.Cost.CrsPerSrc.High.ToDecimal(TokenConstants.Cirrus.Decimals),
                    Low = snapshot.Cost.CrsPerSrc.Low.ToDecimal(TokenConstants.Cirrus.Decimals),
                    Close = snapshot.Cost.CrsPerSrc.Close.ToDecimal(TokenConstants.Cirrus.Decimals)
                },
                SrcPerCrs = new OhlcDto<FixedDecimal>
                {
                    Open = snapshot.Cost.SrcPerCrs.Open.ToDecimal(srcToken.Decimals),
                    High = snapshot.Cost.SrcPerCrs.High.ToDecimal(srcToken.Decimals),
                    Low = snapshot.Cost.SrcPerCrs.Low.ToDecimal(srcToken.Decimals),
                    Close = snapshot.Cost.SrcPerCrs.Close.ToDecimal(srcToken.Decimals)
                }
            },
            SnapshotTypeId = (int)snapshot.SnapshotType,
            Timestamp = snapshot.StartDate,
        });
    }
}